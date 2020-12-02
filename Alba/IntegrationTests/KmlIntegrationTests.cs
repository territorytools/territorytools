using AlbaClient.Kml;
using System.Linq;
using TerritoryTools.IntegrationTestFramework;

namespace Alba.IntegrationTests
{
    [TestFixture]
    public class KmlIntegrationTests
    {
        [Test]
        public void LoadAndConvert_WithRealTestKmlFile_CheckVertices()
        {
            var kml = new KmlGateway().Load("AlbaTest.kml");

            var territories = new KmlToTerritoryConverter().TerritoryListFrom(kml);

            Assert.AreEqual("TEST123-456", territories.First().Number, "Territory Number");

            Assert.AreEqual(22, territories.First().Border.Vertices.Count, "Vertex Count");

            Assert.AreEqual(-122.2272, territories.First().Border.Vertices[0].Longitude, 0.001, "First Vertex Longitude");
            Assert.AreEqual(47.7767, territories.First().Border.Vertices[0].Latitude, 0.001, "First Vertex Latitude");
            Assert.AreEqual(-122.2272, territories.First().Border.Vertices[1].Longitude, 0.001, "Second Vertex Longitude");
            Assert.AreEqual(47.7671, territories.First().Border.Vertices[1].Latitude, 0.001, "Second Vertex Latitude");
        }
    }
}
