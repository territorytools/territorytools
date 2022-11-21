using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class TerritoryResultParser
    {
        public static List<TerritoryDetail> Parse(string value)
        {
            var borders = TerritoryBorderResultParser.Parse(value);

            var territories = new List<TerritoryDetail>();
            foreach (var border in borders)
            {
                territories.Add(TerritoryFrom(border));
            }

            return territories; 
        }

        public static TerritoryDetail TerritoryFrom(AlbaTerritoryBorder border) 
        {
            try
            {
                return TryTerritoryFrom(border);
            }
            catch (Exception e)
            {
                throw new Exception($"Error parsing territory summary: {e.Message}", e);
            }
        }

        static TerritoryDetail TryTerritoryFrom(AlbaTerritoryBorder border)
        {
            var newTerritory = new TerritoryDetail(border.Id)
            {
                Number = border.Number,
                Description = border.Description,
                Notes = border.Notes,
                CountOfAddresses = border.CountOfAddresses,
                Border = border.Border
            };

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

            return newTerritory;
        }
    }
}
