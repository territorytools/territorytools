﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using TerritoryTools.Alba.Controllers.Models;
using System.Text.RegularExpressions;

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
            var borders = nodes.SelectToken("data.borders") as JObject;
            var territories = new List<Territory>();

            foreach (var property in borders.Properties())
                territories.Add(TerritoryFrom(property));

            return territories; 
        }

        private static Territory TerritoryFrom(JProperty property) 
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
                    newTerritory.Number = text;
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
    }
}
