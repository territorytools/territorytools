using HtmlAgilityPack;
using System.Linq;

namespace Controllers.UseCases
{
    public class ExtractAuthK1
    {
        public static string ExtractFrom(string html)
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var node = doc.DocumentNode.SelectNodes("//input[@name='k1']");

           return node?.First()?.GetAttributeValue("value", string.Empty) 
                ?? string.Empty;
        }
    }
}
