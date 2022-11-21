using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class TerritoryBorderResultParser
    {
        const string SignedOutMessaged = "Sorry, you have been signed out.";

        public static List<AlbaTerritoryBorder> Parse(string value)
        {
            if(value.StartsWith(SignedOutMessaged))
            {
                throw new Exception(SignedOutMessaged);
            }

            JsonElement data = JsonDocument.Parse(value).RootElement.GetProperty("data");
            Dictionary<int, TerritoryValues> territories = TerritoriesFrom(data
                .GetProperty("html")
                .GetProperty("territories")
                .GetRawText());

            JsonElement dataBorders = data.GetProperty("borders");
            var borders = new List<AlbaTerritoryBorder>();
            foreach (JsonProperty property in dataBorders.EnumerateObject())
            {
                AlbaTerritoryBorder border = BordersFrom(property);

                // Merge in values from dataBorders
                TerritoryValues territory = territories[border.Id];
                border.Number = territory.Number;
                border.Description = territory.Description;
                border.Notes = territory.Notes;

                borders.Add(border);
            }

            return borders; 
        }

        static AlbaTerritoryBorder BordersFrom(JsonProperty property) 
        {
            try
            {
                Border border = JsonSerializer
                    .Deserialize<Border>(property.Value.GetRawText());

                int.TryParse(property.Name, out int id);
                int.TryParse(border.num, out int count);
                var newTerritory = new AlbaTerritoryBorder()
                {
                    Id = id,
                    CountOfAddresses = count
                };


                foreach (float[] coord in border.pl)
                    newTerritory.Border.Vertices.Add(new Vertex(coord[0], coord[1]));

                return newTerritory;
            }
            catch(Exception e)
            {
                string beginning = property.Value.ToString();
                throw new Exception($"Error parsing border at {beginning.Substring(0, 256)}", e);
            }
        }

        static Dictionary<int, TerritoryValues> TerritoriesFrom(string html)
        {
            try
            {
                return TryTerritoriesFrom(html);
            }
            catch(Exception e)
            {
                throw new Exception($"Error parsing territories HTML: {e.Message}", e);
            }
        }

        static Dictionary<int, TerritoryValues> TryTerritoriesFrom(string html)
        {
            var assignments = new Dictionary<int, TerritoryValues>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rowNodes = doc.DocumentNode.SelectNodes("//tr");
            if (rowNodes != null)
            {
                foreach (HtmlNode rowNode in rowNodes)
                {
                    var territory = TerritoryFrom(rowNode);
                    assignments[territory.Id] = territory;
                }
            }

            return assignments;
        }

        static TerritoryValues TerritoryFrom(HtmlNode rowNode)
        {
            try
            {
                return TryTerritoryFrom(rowNode);
            }
            catch (Exception e)
            {
                throw new Exception($"Error parsing territory HTML: Message: {e.Message} Html: {rowNode.InnerHtml}", e);
            }
        }

        static TerritoryValues TryTerritoryFrom(HtmlNode rowNode)
        {
            int id = 0;
            string number = string.Empty;
            string description = string.Empty;
            string notes = string.Empty;

            var colNodes = rowNode.SelectNodes("td");
            if (colNodes != null)
            {
                for (int col = 0; col < colNodes.Count; col++)
                {
                    HtmlNode colNode = colNodes[col];
                    switch (col)
                    {
                        case 0:
                            int.TryParse(colNode.InnerText, out id);
                            break;
                        case 1:
                            number = colNode.SelectSingleNode("strong").InnerText;
                            description = colNode.GetDirectInnerText().Trim();
                            notes = colNode.SelectSingleNode("small")?.InnerText;
                            break;
                        // Skip column 2 which has the address count because
                        // the JSON already has the same value.
                    }
                }
            }

            return new TerritoryValues { 
                Id = id, 
                Number = number, 
                Description = description, 
                Notes = notes };
        }

        class TerritoryValues
        {
            public int Id;
            public string Number;
            public string Description;
            public string Notes;
        }
    }
}
