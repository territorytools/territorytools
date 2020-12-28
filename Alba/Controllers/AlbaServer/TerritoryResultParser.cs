using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using TerritoryTools.Alba.Controllers.Models;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class TerritoryResultParser
    {
        public static List<Territory> Parse(string value)
        {
            if(value.StartsWith("Sorry, you have been signed out."))
            {
                throw new Exception("Sorry, you have been signed out.");
            }

            var nodes = JObject.Parse(value);
            var htmlObject = nodes.SelectToken("data.html") as JObject;
            var territories = TerritoriesFrom(htmlObject.GetValue("territories")?.ToString());
            
            var bordersObject = nodes.SelectToken("data.borders") as JObject;
            var borders = new List<Territory>();
            foreach (var property in bordersObject.Properties())
            {
                var border = BordersFrom(property);
                if (int.TryParse(border.Id, out int id))
                {
                    var territory = territories[id];
                    border.Number = territory.Number;
                    border.Description = territory.Description;
                    border.Notes = territory.Notes;
                    borders.Add(border);
                }
            }

            return borders; 
        }

        static Territory BordersFrom(JProperty property) 
        {
            try
            {
                var border = JsonConvert.DeserializeObject<Border>(property.Value.ToString());
                var newTerritory = new Territory(property.Name)
                {
                    CountOfAddresses = border.num
                };

                string text = border.tt;

                var regex = new Regex(@"([^<>]+) ([^<>]+)");
                var matches = regex.Match(text);
                if(matches.Success && matches.Groups.Count == 2)
                {
                    newTerritory.Number = matches.Groups[1].Value;
                    newTerritory.Description = matches.Groups[2].Value;
                }
                else
                {
                    newTerritory.Description = text;
                }

                newTerritory.CityArea = !string.IsNullOrWhiteSpace(newTerritory.Description) 
                        && newTerritory.Description.Trim().Length == 6
                    ? newTerritory.Description.Substring(0, 6)
                    : string.Empty;

                newTerritory.CityCode = !string.IsNullOrWhiteSpace(newTerritory.CityArea)
                        && newTerritory.CityArea.Length == 6
                    ? newTerritory.CityArea.Substring(0, 3)
                    : string.Empty;

                newTerritory.ZipCodeSuffix = !string.IsNullOrWhiteSpace(newTerritory.CityArea)
                        && newTerritory.CityArea.Length == 6
                    ? newTerritory.CityArea.Substring(3, 3)
                    : string.Empty;

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

        static Dictionary<
                int, 
                (int Id, string Number, string Description, string Notes)> 
            TerritoriesFrom(string html)
        {
            try
            {
                return TryTerritoriesParse(html);
            }
            catch(Exception e)
            {
                throw new Exception($"Error parsing territories HTML: {e.Message}", e);
            }
        }

        static Dictionary<
                int,
                (int Id, string Number, string Description, string Notes)> 
            TryTerritoriesParse(string html)
        {
            var assignments = new Dictionary<
                int,
                (int Id, string Number, string Description, string Notes)>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var rowNodes = doc.DocumentNode.SelectNodes("//tr");

            if (rowNodes != null)
            {
                int row = 0;
                foreach (HtmlNode rowNode in rowNodes)
                {
                    var territory = TerritoryFrom(rowNode);
                    assignments[territory.Id] = territory;

                    row++;
                }
            }

            return assignments;
        }

        static (int Id, string Number, string Description, string Notes)
            TerritoryFrom(HtmlNode rowNode)
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

        static (int Id, string Number, string Description, string Notes)
            TryTerritoryFrom(HtmlNode rowNode)
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
                    }
                }

            }

            return (Id: id, Number: number, Description: description, Notes: notes);
        }
    }
}
