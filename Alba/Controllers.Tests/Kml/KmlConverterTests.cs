using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Alba.Controllers.Models;
using Alba.Controllers.Kml;

namespace AlbaClient.Tests.KmlGateways
{
    [TestFixture]
    public class KmlConverterTests
    {
        [Test]
        public void Convert_WithTwoCoordinatesInFolder_ReturnsTwoBorderVertices()
        {
            GoogleMapsKml kml = DefaultKml();

            kml.Document.Folder.First().Placemark = PlacemarkWithTwoCoordinates();

            var territories = new KmlToTerritoryConverter().TerritoryListFrom(kml);

            AssertTwoCoordinatesAreNowVertices(territories);
        }

        [Test]
        public void Convert_WithTwoCoordinatesInPlacemark_ReturnsTwoBorderVertices()
        {
            GoogleMapsKml kml = DefaultKml();

            kml.Document.Placemark = PlacemarkWithTwoCoordinates();

            var territories = new KmlToTerritoryConverter().TerritoryListFrom(kml);

            AssertTwoCoordinatesAreNowVertices(territories);
        }

        [Test]
        public void Convert_WithTerritoryName_NameIsSet()
        {
            GoogleMapsKml kml = DefaultKml();
            kml.Document.Placemark = PlacemarkWithTwoCoordinates();

            var territories = new KmlToTerritoryConverter().TerritoryListFrom(kml);

            Assert.AreEqual("TEST321-654", territories.First().Number);
        }

        [Test]
        public void Convert_WithNullPolygon_IsSkipped()
        {
            GoogleMapsKml kml = DefaultKml();
            kml.Document.Placemark = PlacemarkWithTwoCoordinates();

            kml.Document.Placemark[0].MultiGeometry.Polygon = null;
            AssertSkipped(kml);

            kml.Document.Placemark[0].MultiGeometry = null;
            AssertSkipped(kml);

            kml.Document.Placemark[0].Polygon = null;
            AssertSkipped(kml);

        }

        [Test]
        public void Convert_WithSmallTerritory_IsNotNull()
        {
            var territories = new List<Territory>()
            {
                new Territory("test id")
                {
                    Number = "test number",
                    Border = new Border()
                    {
                        Vertices = new List<Vertex>()
                          {
                               new Vertex(1.23, 4.56),
                               new Vertex(2.34, 6.78)
                          }
                    }
                }
            };

            var kml = new TerritoryToKmlConverter().KmlFrom(territories);

            Assert.IsNotNull(kml);
        }

        [Test]
        public void Convert_WithSmallTerritory_DescriptionIsSet()
        {
            var territories = new List<Territory>()
            {
                new Territory("test id")
                {
                    Number = "test number",
                    Description = "test description",
                    Border = new Border()
                    {
                        Vertices = new List<Vertex>()
                          {
                               new Vertex(1.23, 4.56),
                               new Vertex(2.34, 6.78)
                          }
                    }
                }
            };

            var kml = new TerritoryToKmlConverter().KmlFrom(territories);

            Assert.AreEqual("test description", kml.Document.Folder[0].Placemark[0].description);
        }

        private static void AssertSkipped(GoogleMapsKml kml)
        {
            var territories2 = new KmlToTerritoryConverter().TerritoryListFrom(kml);

            Assert.AreEqual("TEST321-654", territories2.First().Number);
        }

        private static void AssertTwoCoordinatesAreNowVertices(IEnumerable<Territory> territories)
        {
            Assert.AreEqual(2, territories.First().Border.Vertices.Count);
            Assert.AreEqual(11.11, territories.First().Border.Vertices[0].Longitude);
            Assert.AreEqual(22.22, territories.First().Border.Vertices[0].Latitude);
            Assert.AreEqual(33.33, territories.First().Border.Vertices[1].Longitude);
            Assert.AreEqual(44.44, territories.First().Border.Vertices[1].Latitude);
        }

        private static GoogleMapsKml DefaultKml()
        {
            return new GoogleMapsKml()
            {
                Document = new Document()
                {
                    Folder = new DocumentFolder[]
                    {
                        new DocumentFolder()
                    }
                }
            };
        }

        private static Placemark[] PlacemarkWithTwoCoordinates()
        {
            return new Placemark[] {
                new Placemark()
                {
                    name = "TEST321-654",
                    MultiGeometry = new MultiGeometry()
                    {
                        Polygon = new PlacemarkPolygon[]
                        {
                            new PlacemarkPolygon()
                            {
                                outerBoundaryIs = new OuterBoundaryIs()
                                {
                                    LinearRing = new LinearRing()
                                    {
                                        coordinates = "11.11,22.22 33.33,44.44"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
