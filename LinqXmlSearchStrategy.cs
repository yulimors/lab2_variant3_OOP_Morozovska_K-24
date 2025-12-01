using System.Xml.Linq;
using Lab2Xml.Models;

namespace Lab2Xml.Services
{
    public class LinqSearchStrategy : IXmlSearchStrategy
    {
        public List<Student> Search(string filePath, Student template)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return new List<Student>();

            try
            {
                var doc = XDocument.Load(filePath);
                if (doc.Root == null) return new List<Student>();

                return doc.Descendants("Student")
                    .Select(x => new Student
                    {
                        Name = (string?)x.Attribute("Name") ?? string.Empty,
                        Faculty = (string?)x.Attribute("Faculty") ?? string.Empty,
                        Department = (string?)x.Attribute("Department") ?? string.Empty,
                        Course = (string?)x.Attribute("Course") ?? string.Empty,
                        Room = (string?)x.Attribute("Room") ?? string.Empty,
                        Phone = (string?)x.Attribute("Phone") ?? string.Empty
                    })
                    .Where(s => IsMatch(s, template))
                    .ToList();
            }
            catch
            {
                return new List<Student>();
            }
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