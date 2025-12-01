namespace Lab2Xml.Models
{
    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string DisplayInfo => $"{Name} ({Faculty}, {Course} курс) — {Room}";
    }
}