using TerritoryTools.Alba.Controllers.Models;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.Kml
{
    public class PlacemarkConverterToAlbaTerritoryBorder
    {
        public static AlbaTerritoryBorder From(Placemark placemark)
        {
            return new PlacemarkConverterToAlbaTerritoryBorder().TerritoryFrom(placemark);
        }

        public static string ColorString(Color color)
        {
            return $"{color.A.ToString("X2")}"
                 + $"{color.Blue.ToString("X2")}"
                 + $"{color.Green.ToString("X2")}"
                 + $"{color.Red.ToString("X2")}";
        }

        public static readonly Color Green = new Color { Red = 0, Green = 255, Blue = 0, A = 128 };

        public Placemark PlacemarkFrom(AlbaTerritoryBorder territory)
        {
            return new Placemark()
            {
                name = territory.Number,
                description = territory.Description,
                styleUrl =$"#t-fill-color-{ColorString(Green)}",
                MultiGeometry = MultiGeometryFrom(territory.Border),
                ExtendedData = ExtendedDataFrom(territory)
            };
        }

        AlbaTerritoryBorder TerritoryFrom(Placemark placemark)
        {
            int.TryParse(placemark.name, out int id);
            int.TryParse(FromExtendedData(placemark, "CountOfAddresses"), out int count);

            return new AlbaTerritoryBorder()
            {
                Id = id,
                Number = placemark?.name,
                Description = placemark?.description,
                Border = BorderFrom(placemark),
                Notes = FromExtendedData(placemark, "Notes"),
                CountOfAddresses = count
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

        string FromExtendedData(Placemark placemark, string name)
        {
            if(placemark?.ExtendedData?.Data == null)
            {
                return null;
            }

            foreach (Data data in placemark.ExtendedData.Data)
            {
                if(data.name == name)
                {
                    return data.value;
                }
            }

            return null;
        }

        ExtendedData ExtendedDataFrom(AlbaTerritoryBorder territory)
        {
            return new ExtendedData
            {
                Data = new Data[]
                {
                    new Data { name = "Number", value = territory.Number},
                    new Data { name = "AssignUrl", value = $"http://territorytools.org/t/{territory.Number}"},
                    new Data { name = "CountOfAddresses", value = territory.CountOfAddresses.ToString()},
                    new Data { name = "Description", value = territory.Description},
                    new Data { name = "Notes", value = territory.Notes},
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
