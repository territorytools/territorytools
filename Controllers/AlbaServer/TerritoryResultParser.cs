using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using AlbaClient.Models;

namespace AlbaClient.AlbaServer
{
    public class TerritoryResultParser
    {
        public static List<Territory> Parse(string value)
        {
            var nodes = JObject.Parse(value);
            var borders = nodes.SelectToken("data.borders") as JObject;
            var territories = new List<Territory>();

            foreach (var property in borders.Properties())
                territories.Add(TerritoryFrom(property));

            return territories; 
        }

        private static Territory TerritoryFrom(JProperty property) 
        {
            var border = JsonConvert.DeserializeObject<Border>(property.Value.ToString());
            var newTerritory = new Territory(property.Name)
            {
                Number = border.tt.Substring(0, border.tt.IndexOf(" ")),
                CountOfAddresses = border.num,
                Description = border.tt.Substring(border.tt.IndexOf(" ") + 1),
                CityArea = border.tt.Substring(border.tt.IndexOf(" ") + 1).Substring(0, 6),
                CityCode = border.tt.Substring(border.tt.IndexOf(" ") + 1).Substring(0, 3),
                ZipCodeSuffix = border.tt.Substring(border.tt.IndexOf(" ") + 1).Substring(3, 3)
            };

            foreach (float[] coord in border.pl)
                newTerritory.Border.Vertices.Add(new Vertex(coord[0], coord[1]));

            return newTerritory;
        }
    }
}
