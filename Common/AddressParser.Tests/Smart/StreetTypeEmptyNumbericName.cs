using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class StreetTypeEmptyNumericName : ParserTestBase
    {
        [TestCase("123 112th Bellevue WA 98004", "WA")]
        public void Region(string text, string region)
        {
            Assert.AreEqual(region, Test(text).Region.Code);
        }

        [TestCase("123 112th Bellevue WA 98004", "Bellevue")]
        public void City(string text, string city)
        {
            Assert.AreEqual(city, Test(text).City.Name);
        }

        [TestCase("123 112th Bellevue WA 98004", "112th")]
        public void StreetName(string text, string name)
        {
            Assert.AreEqual(name, Test(text).Street.Name.Name);
        }

        [TestCase("123 112th Bellevue WA 98004", "")]
        public void StreetType(string text, string type)
        {
            Assert.AreEqual(type, Test(text).Street.Name.StreetType);
        }

        [TestCase("123 112th Bellevue WA 98004", "112th", "", "Bellevue", "WA")]
        public void OneWordCityAndRegion(
            string text,
            string name,
            string type,
            string city,
            string region)
        {
            Assert.AreEqual(name, Test(text).Street.Name.Name.ToString());
            Assert.AreEqual(type, Test(text).Street.Name.StreetType.ToString());
            Assert.AreEqual(city, Test(text).City.Name);
            Assert.AreEqual(region, Test(text).Region.Code);
        }
    }
}
