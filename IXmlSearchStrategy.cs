using Lab2Xml.Models;

namespace Lab2Xml.Services
{
    public interface IXmlSearchStrategy
    {
        List<Student> Search(string filePath, Student searchTemplate);
    }
}