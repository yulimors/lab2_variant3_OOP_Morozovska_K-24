using System.Xml;
using Lab2Xml.Models;

namespace Lab2Xml.Services
{
    public class SaxSearchStrategy : IXmlSearchStrategy
    {
        public List<Student> Search(string filePath, Student template)
        {
            var results = new List<Student>();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return results;

            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            try
            {
                using (var reader = XmlReader.Create(filePath, settings))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                        {
                            var student = new Student
                            {
                                Name = reader.GetAttribute("Name") ?? string.Empty,
                                Faculty = reader.GetAttribute("Faculty") ?? string.Empty,
                                Department = reader.GetAttribute("Department") ?? string.Empty,
                                Course = reader.GetAttribute("Course") ?? string.Empty,
                                Room = reader.GetAttribute("Room") ?? string.Empty,
                                Phone = reader.GetAttribute("Phone") ?? string.Empty
                            };

                            if (IsMatch(student, template))
                            {
                                results.Add(student);
                            }
                        }
                    }
                }
            }
            catch 
            { 
            
            }

            return results;
        }

        private bool IsMatch(Student current, Student template)
        {
            if (!string.IsNullOrEmpty(template.Name) &&
                !current.Name.Contains(template.Name, StringComparison.OrdinalIgnoreCase)) return false;

            if (!string.IsNullOrEmpty(template.Faculty) &&
                !current.Faculty.Equals(template.Faculty, StringComparison.OrdinalIgnoreCase)) return false;

            if (!string.IsNullOrEmpty(template.Course) &&
                !current.Course.Equals(template.Course, StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }
    }
}