using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Xsl;
using Lab2Xml.Models;
using Lab2Xml.Services;

namespace Lab2Xml.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private string _xmlPath = string.Empty;
        private string _xslPath = string.Empty;

        private string _keywordName = string.Empty;
        private string _selectedFaculty = null;
        private string _selectedCourse = null;
        private string _selectedStrategyName = "LINQ to XML";

        private string _statusMessage = "Готовий до роботи";
        private Color _statusColor = Colors.Gray;
        private bool _isClearEnabled = false;

        public ObservableCollection<Student> Results { get; set; } = new();
        public ObservableCollection<string> Faculties { get; set; } = new();
        public ObservableCollection<string> Courses { get; set; } = new();
        public ObservableCollection<string> Strategies { get; set; } = new() { "SAX API", "DOM API", "LINQ to XML" };


        public string KeywordName
        {
            get => _keywordName;
            set
            {
                _keywordName = value ?? string.Empty;
                OnPropertyChanged();
                UpdateButtonState();
            }
        }

        public string SelectedFaculty
        {
            get => _selectedFaculty;
            set
            {
                _selectedFaculty = value;
                OnPropertyChanged();
                UpdateButtonState();
            }
        }

        public string SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                _selectedCourse = value;
                OnPropertyChanged();
                UpdateButtonState();
            }
        }

        public string SelectedStrategyName
        {
            get => _selectedStrategyName;
            set { if (!string.IsNullOrEmpty(value)) { _selectedStrategyName = value; OnPropertyChanged(); } }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public Color StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        public bool IsClearEnabled
        {
            get => _isClearEnabled;
            set { _isClearEnabled = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand TransformCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel()
        {
            Courses.Add("1"); Courses.Add("2"); Courses.Add("3");
            Courses.Add("4"); Courses.Add("5"); Courses.Add("6");

            SearchCommand = new Command(async () => await PerformSearch());
            ClearCommand = new Command(PerformClear);
            TransformCommand = new Command(async () => await PerformTransform());
            ExitCommand = new Command(async () => await PerformExit());

            Task.Run(async () => await LoadInitialData());
        }

        private void UpdateButtonState()
        {
            bool shouldEnable = !string.IsNullOrEmpty(KeywordName) ||
                                SelectedFaculty != null ||
                                SelectedCourse != null ||
                                Results.Count > 0;

            IsClearEnabled = shouldEnable;
        }

        private async Task LoadInitialData()
        {
            try
            {
                SetStatus("Завантаження даних...", Colors.Orange);
                _xmlPath = await CopyResourceToFile("dormitory.xml");
                _xslPath = await CopyResourceToFile("transform.xsl");

                if (string.IsNullOrEmpty(_xmlPath)) return;

                var strategy = new LinqSearchStrategy();
                var allStudents = strategy.Search(_xmlPath, new Student());

                var uniqueFaculties = allStudents
                    .Select(s => s.Faculty)
                    .Where(f => !string.IsNullOrEmpty(f))
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Faculties.Clear();
                    foreach (var f in uniqueFaculties) Faculties.Add(f);
                    SetStatus("Дані успішно завантажено", Colors.Green);
                });
            }
            catch (Exception ex)
            {
                SetStatus($"Помилка БД: {ex.Message}", Colors.Red);
            }
        }

        private async Task PerformSearch()
        {
            if (string.IsNullOrEmpty(_xmlPath) || !File.Exists(_xmlPath))
            {
                SetStatus("База даних не знайдена!", Colors.Red);
                return;
            }

            try
            {
                Results.Clear();
                SetStatus("Виконується пошук...", Colors.Orange);

                IXmlSearchStrategy strategy = SelectedStrategyName switch
                {
                    "SAX API" => new SaxSearchStrategy(),
                    "DOM API" => new DomSearchStrategy(),
                    "LINQ to XML" => new LinqSearchStrategy(),
                    _ => new LinqSearchStrategy()
                };

                var template = new Student
                {
                    Name = KeywordName.Trim(),
                    Faculty = SelectedFaculty ?? string.Empty,
                    Course = SelectedCourse ?? string.Empty
                };

                var found = await Task.Run(() => strategy.Search(_xmlPath, template));

                foreach (var item in found) Results.Add(item);

                if (found.Count == 0)
                    SetStatus("За вашим запитом нічого не знайдено", Colors.Red);
                else
                    SetStatus($"Знайдено студентів: {found.Count}", Colors.Green);

                UpdateButtonState();
            }
            catch (Exception ex)
            {
                SetStatus($"Помилка пошуку: {ex.Message}", Colors.Red);
            }
        }

        private void PerformClear()
        {
            KeywordName = string.Empty;
            SelectedFaculty = null;
            SelectedCourse = null;
            Results.Clear();
            SetStatus("Фільтри очищено", Colors.Gray);

            UpdateButtonState();
        }


        private async Task PerformTransform()
        {
            try
            {
                var xslCompiled = new XslCompiledTransform();
                xslCompiled.Load(_xslPath);
                string htmlPath = Path.Combine(FileSystem.CacheDirectory, "report.html");

                await Task.Run(() => xslCompiled.Transform(_xmlPath, htmlPath));
                SetStatus("HTML успішно створено!", Colors.Green);

                if (Application.Current?.MainPage != null)
                {
                    bool open = await Application.Current.MainPage.DisplayAlert("Успіх", "Відкрити звіт?", "Так", "Ні");
                    if (open) await Launcher.Default.OpenAsync(new OpenFileRequest("Звіт", new ReadOnlyFile(htmlPath)));
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Помилка HTML: {ex.Message}", Colors.Red);
            }
        }

        private async Task PerformExit()
        {
            if (Application.Current?.MainPage != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Вихід", "Завершити роботу?", "Так", "Ні");
                if (answer) Application.Current.Quit();
            }
        }

        private async Task<string> CopyResourceToFile(string filename)
        {
            try
            {
                string targetFile = Path.Combine(FileSystem.CacheDirectory, filename);
                using Stream stream = await FileSystem.OpenAppPackageFileAsync(filename);
                using FileStream outputStream = File.Create(targetFile);
                await stream.CopyToAsync(outputStream);
                return targetFile;
            }
            catch { return string.Empty; }
        }

        private void SetStatus(string message, Color color)
        {
            StatusMessage = message;
            StatusColor = color;
        }
    }
}
