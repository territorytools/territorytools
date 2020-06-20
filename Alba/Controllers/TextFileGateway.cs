using System.IO;

namespace Alba.Controllers
{
    public class TextFileGateway
    {
        public static void Save(string fileName, string text)
        {
            File.WriteAllText(fileName, text);
        }
    }
}
