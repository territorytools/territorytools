using AlbaClient.Kml;
using System.IO;
using TerritoryTools.IntegrationTestFramework;

namespace Alba.IntegrationTests
{
    [TestFixture]
    public class KmlGatewayIntegrationTests
    {
        [Test]
        public void Deserialize_WithFile_NoException()
        {
            new KmlGateway().Load("AlbaTest.kml");
        }

        [Test]
        public void Deserialize_WithNoFile_ThrowsException ()
        {
            try {
                new KmlGateway ().Load ("Not_An_Existing_File.kml");
                Assert.Fail ("Exception should have been thrown.");
            } catch (FileNotFoundException) {
            }
        }

        [Test]
        public void Deserialize_WithFile_CheckFirstNameValue()
        {
            var kml = new KmlGateway().Load("AlbaTest.kml");

            Assert.AreEqual("TEST123-456", kml.Document.Folder[0].Placemark[0].name);
        }

        [Test]
        public void Save_WithSmallKml_FileExists()
        {
            var kml = new GoogleMapsKml()
            {
                Document = new Document()
                {
                    name = "test name"
                }
            };

            new KmlGateway().Save("OutputTest.kml", kml);

            Assert.AreEqual(true, File.Exists("OutputTest.kml"));
        }

        [Test]
        public void Save_WithSmallKml_NameMatches()
        {
            string expected = @"<name>test name</name>";
            File.Delete("OutputTest.kml");

            var kml = new GoogleMapsKml()
            {
                Document = new Document()
                {
                    name = "test name"
                }
            };

            new KmlGateway().Save("OutputTest.kml", kml);

            string text = File.ReadAllText("OutputTest.kml");

            Assert.AreEqual(true, text.Contains(expected));
        }
    }
}
