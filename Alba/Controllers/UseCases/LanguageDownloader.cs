using TerritoryTools.Alba.Controllers.AlbaServer;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class AlbaLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LanguageDownloader
    {
        private AlbaConnection client;

        public LanguageDownloader(AlbaConnection client)
        {
            this.client = client;
        }

        public List<AlbaLanguage> GetLanguages()
        {
            var html = client.DownloadString(
              RelativeUrlBuilder.GetLanguages());

            return ExtractLanguages(html);
        }

        public void SaveAs(string path)
        {
            var html = client.DownloadString(
                RelativeUrlBuilder.GetLanguages());

            File.WriteAllText(path, html);
        }

        public static List<AlbaLanguage> LoadLanguagesFrom(string path)
        {
            string html = File.ReadAllText(path);

            return ExtractLanguages(html);
        }

        public static List<AlbaLanguage> ExtractLanguages(string html)
        {
            var languages = new List<AlbaLanguage>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rowNodes = doc
                .DocumentNode
                .SelectNodes("//optgroup[@label='Language']/option");

            if (rowNodes != null)
            {
                foreach (HtmlNode rowNode in rowNodes)
                {
                    string idString = rowNode.GetAttributeValue("value", "0");

                    int.TryParse(idString, out int id);

                    var language = new AlbaLanguage
                    {
                        Id = id,
                        Name = rowNode.InnerText
                    };

                    languages.Add(language);
                }
            }

            return languages;
        }
    }
}
