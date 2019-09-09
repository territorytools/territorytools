using System.IO;
using System.Reflection;

namespace AlbaClient.Tests
{
    public class TestFileProvider
    {
        public static string ContentOfTestFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}
