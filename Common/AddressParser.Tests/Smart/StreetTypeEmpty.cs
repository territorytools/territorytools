using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class StreetTypeEmpty : ParserTestBase
    {
        [TestCase("123 Broadway Bellevue WA", "WA")]
        [TestCase("123 Broadway Bellevue WA 98004", "WA")]
        [TestCase("123 112th Bellevue WA 98004", "WA")]
        [TestCase("123 Main South Bend WA", "WA")]
        [TestCase("123 Main North Bend WA", "WA")]
        public void Region(string text, string region)
        {
            Assert.AreEqual(region, Test(text).Region.Code);
        }

        [TestCase("123 Broadway Bellevue WA", "Bellevue")]
        [TestCase("123 Broadway Bellevue WA 98004", "Bellevue")]
        [TestCase("123 112th Bellevue WA 98004", "Bellevue")]
        [TestCase("123 Main South Bend WA", "Bend")]
        [TestCase("123 Main North Bend WA", "North Bend")]
        public void City(string text, string city)
        {
            Assert.AreEqual(city, Test(text).City.Name);
        }

        [TestCase("123 Broadway Bellevue WA", "Broadway")]
        [TestCase("123 Broadway Bellevue WA 98004", "Broadway")]
        [TestCase("123 112th Bellevue WA 98004", "112th")]
        [TestCase("123 Main South Bend WA", "Main South")]
        public void StreetName(string text, string name)
        {
            Assert.AreEqual(name, Test(text).Street.Name.Name);
        }

        [TestCase("123 Broadway Bellevue WA", "")]
        [TestCase("123 Broadway Bellevue WA 98004", "")]
        [TestCase("123 112th Bellevue WA 98004", "")]
        [TestCase("123 Main South Bend WA", "")]
        public void StreetType(string text, string type)
        {
            Assert.AreEqual(type, Test(text).Street.Name.StreetType);
        }

        [Test]
        public void NoStreetType_TwoWordCityAndRegion()
        {
            string text = "123 Main North Bend WA";
            Assert.AreEqual("Main", Test(text).Street.Name.ToString());
            Assert.AreEqual("North Bend", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [TestCase("123 Broadway Bellevue WA", "Broadway", "", "Bellevue", "WA")]
        [TestCase("123 Broadway Bellevue WA 98004", "Broadway", "", "Bellevue", "WA")]
        [TestCase("123 112th Bellevue WA 98004", "112th", "", "Bellevue", "WA")]
        [TestCase("123 Main South UnkownCity WA", "Main South", "", "UnkownCity", "WA")]
        public void NoStreetType_OneWordCityAndRegion(
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
