using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class CityNameUnknown : ParserTestBase
    {
        [TestCase("123 Main South Bend WA", "Bend")]
        public void CityName(string text, string name)
        {
            Assert.AreEqual(name, Test(text).City.Name);
        }

        [TestCase("123 Main South Bend WA", "Main South")]
        public void StreetName(string text, string name)
        {
            Assert.AreEqual(name, Test(text).Street.Name.ToString());
        }
    }
}
