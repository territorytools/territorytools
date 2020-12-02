using NUnit.Framework;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class IgnoreMissingParts : ParserTestBase
    {
        [TestCase("123 Main St", "")]
        [TestCase("123 Main St WA", "")]
        [TestCase("123 Main St Seattle", "")]
        [TestCase("123 Main St Seattle WA", "")]
        public void Ignored(string text, string missing)
        {
            var address = ParseIgnore(text);
            Assert.AreEqual(missing, address.ErrorMessage);
        }

        [TestCase("123 Main St", "City.Name, Region.Code")]
        //[TestCase("123 Main St WA", "Region.Code")]
        //[TestCase("123 Main St Seattle", "City.Name")]
        //[TestCase("123 Main St Seattle WA", "")]
        public void NotIgnored(string text, string missing)
        {
            var address = ParseNotIgnore(text);
            Assert.AreEqual(missing, address.ErrorMessage);
        }

        [TestCase("123 Cityless Main St")]
        [TestCase("123 Regionless Main St, Seattle")]
        public void NoParseResults(string text)
        {
            var address = ParseNotIgnore(text);
            Assert.IsEmpty(address.Street.Number);
            Assert.IsEmpty(address.Street.Name.Name);
            Assert.IsEmpty(address.Street.Name.StreetType);
            Assert.IsEmpty(address.City.Name);
            //Assert.AreEqual(text, address.FailedAddress);
        }

        Address ParseIgnore(string text)
        {
            var parser = DefaultParser();
            parser.IgnoreMissingCity = true;
            parser.IgnoreMissingRegion = true;
            return parser.Parse(text);
        }

        Address ParseNotIgnore(string text)
        {
            var parser = DefaultParser();
            parser.KeepParseResultsOnError = false;
            return parser.Parse(text);
        }
    }
}
