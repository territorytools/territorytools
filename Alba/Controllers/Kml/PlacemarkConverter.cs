﻿using TerritoryTools.Alba.Controllers.Models;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.Kml
{
    public class PlacemarkConverter
    {
        public static TerritoryDetail From(Placemark placemark)
        {
            return new PlacemarkConverter().TerritoryFrom(placemark);
        }

        public static string ColorString(Color color)
        {
            return $"{color.A.ToString("X2")}"
                 + $"{color.Blue.ToString("X2")}"
                 + $"{color.Green.ToString("X2")}"
                 + $"{color.Red.ToString("X2")}";
        }

        public Placemark PlacemarkFrom(TerritoryDetail territory)
        {
            return new Placemark()
            {
                name = territory.Number,
                description = territory.Description,
                styleUrl =$"#t-fill-color-{ColorString(territory.FillColor)}",
                MultiGeometry = MultiGeometryFrom(territory.Border),
                ExtendedData = ExtendedDataFrom(territory)
            };
        }

        TerritoryDetail TerritoryFrom(Placemark placemark)
        {
            int.TryParse(placemark.name, out int id);

            return new TerritoryDetail(id)
            {
                Number = placemark?.name,
                Description = placemark?.description,
                Border = BorderFrom(placemark),
            };
        }

        Border BorderFrom(Placemark placemark)
        {
            if (placemark?.MultiGeometry?.Polygon != null)
                return BorderFrom(placemark.MultiGeometry.Polygon.FirstOrDefault());
            else if (placemark?.Polygon != null)
                return BorderFrom(placemark.Polygon);
            else
                return null;
        }

        Border BorderFrom(PlacemarkPolygon polygon)
        {
            return new BorderConverter()
                .BorderFrom(polygon.outerBoundaryIs.LinearRing.coordinates);
        }

        MultiGeometry MultiGeometryFrom(Border border)
        {
            return new MultiGeometry()
            {
                Polygon = new PlacemarkPolygon[] { PolygonFrom(border) },
            };
        }

        ExtendedData ExtendedDataFrom(TerritoryDetail territory)
        {
            int density = territory.CountOfAddresses / 10;

            return new ExtendedData
            {
                Data = new Data[]
                {
                    new Data { name = "Number", value = territory.Number},
                    new Data { name = "CityArea", value = territory.CityArea},
                    new Data { name = "AssignUrl", value = $"https://territorytools.org/t/{territory.Number}"},
                    new Data { name = "SignedOut", value = territory.SignedOut?.ToString()},
                    new Data { name = "SignedOutTo", value = territory.SignedOutTo},
                    new Data { name = "Status", value = territory.Status},
                    new Data { name = "LastCompleted", value = territory.LastCompleted?.ToString()},
                    new Data { name = "LastCompletedBy", value = territory.LastCompletedBy},
                    new Data { name = "CountOfAddresses", value = territory.CountOfAddresses.ToString()},
                    new Data { name = "Description", value = territory.Description},
                    new Data { name = "AddressDensity", value = density.ToString()},
                    new Data { name = "MonthsAgoCompleted", value = territory.MonthsAgoCompleted?.ToString()},
                    new Data { name = "YearsAgoCompleted", value = territory.YearsAgoCompleted.ToString()},
                    new Data { name = "NeverCompleted", value = territory.NeverCompleted.ToString()},
                    new Data { name = "MobileLink", value = ""},
                    new Data { name = "Notes", value = territory.Notes},
                    new Data { name = "CityCode", value = territory.CityCode},
                    new Data { name = "ZipCodeSuffix", value = territory.ZipCodeSuffix},
                }
            };
        }

        PlacemarkPolygon PolygonFrom(Border border)
        {
            return new PlacemarkPolygon()
            {
                outerBoundaryIs = new OuterBoundaryIs()
                {
                    LinearRing = new LinearRing()
                    {
                        coordinates = BorderConverter.From(border),
                    }
                }
            };
        }
    }
}
