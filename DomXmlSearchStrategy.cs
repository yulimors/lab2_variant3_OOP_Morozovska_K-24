using System.Xml;
using Lab2Xml.Models;

namespace Lab2Xml.Services
{
    public class DomSearchStrategy : IXmlSearchStrategy
    {
        public List<Student> Search(string filePath, Student template)
        {
            var results = new List<Student>();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return results;

            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);

                var nodes = doc.SelectNodes("//Student");
                if (nodes == null) return results;

                foreach (XmlNode node in nodes)
                {
                    var student = new Student
                    {
                        Name = node.Attributes?["Name"]?.Value ?? string.Empty,
                        Faculty = node.Attributes?["Faculty"]?.Value ?? string.Empty,
                        Department = node.Attributes?["Department"]?.Value ?? string.Empty,
                        Course = node.Attributes?["Course"]?.Value ?? string.Empty,
                        Room = node.Attributes?["Room"]?.Value ?? string.Empty,
                        Phone = node.Attributes?["Phone"]?.Value ?? string.Empty
                    };

                    if (IsMatch(student, template))
                    {
                        results.Add(student);
                    }
                }
            }
            catch { /* Обробка помилок */ }

            return results;
        }

        private bool IsMatch(Student current, Student template)
        {
            if (!string.IsNullOrEmpty(template.Name) && !current.Name.Contains(template.Name, StringComparison.OrdinalIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(template.Faculty) && !current.Faculty.Equals(template.Faculty, StringComparison.OrdinalIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(template.Course) && !current.Course.Equals(template.Course, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }
    }
}