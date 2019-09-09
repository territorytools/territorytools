using NUnit.Framework;
using AlbaClient.Kml;

namespace AlbaClient.Tests.Kml
{
    [TestFixture]
    public class TerritoryConverterTests
    {
        [Test]
        public void From_Placemark_WithDescriptionAndNameSet_ReturnsTerritoryNoAndCode()
        {
            var placemark = new Placemark()
            {
                description = "test description",
                name = "test name"
            };

            var territory = PlacemarkConverter.From(placemark);

            Assert.AreEqual("test description", territory.Description);
            Assert.AreEqual("test name", territory.Number);
        }
    }
}
