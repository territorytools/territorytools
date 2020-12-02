using System.IO;

namespace TerritoryTools.Alba.Controllers
{
    public class TextFileGateway
    {
        public static void Save(string fileName, string text)
        {
            File.WriteAllText(fileName, text);
        }
    }
}
