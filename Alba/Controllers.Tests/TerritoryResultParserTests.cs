using NUnit.Framework;
using Alba.Controllers.Models;
using Alba.Controllers.AlbaServer;

namespace AlbaClient.Tests
{
    [TestFixture]
    public class TerritoryResultParserTests
    {
        [Test]
        public void Parse_WithTestFile_ReturnsCorrectIdCountAndVertices()
        {
            string json = TestFileProvider.ContentOfTestFile("AllTerritories.json.txt");

            var territories = TerritoryResultParser.Parse(json);

            Assert.AreEqual("36950", territories[0].Id);
            Assert.AreEqual("17", territories[0].CountOfAddresses);

            VerticesAreEqual(new Vertex(47.566613, -122.276787), territories[0].Border.Vertices[0]);
            VerticesAreEqual(new Vertex(47.557899, -122.274213), territories[0].Border.Vertices[1]);

            Assert.AreEqual("67186", territories[1].Id);
            Assert.AreEqual("19", territories[1].CountOfAddresses);
        }

        private void VerticesAreEqual(Vertex vertexA, Vertex vertexB)
        {
            Assert.AreEqual(vertexA.Latitude, vertexB.Latitude, 0.001);
            Assert.AreEqual(vertexA.Longitude, vertexB.Longitude, 0.001);
        }
    }
}
