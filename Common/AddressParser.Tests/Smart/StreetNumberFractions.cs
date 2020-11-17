using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class StreetNumberFractions : ParserTestBase
    {
        [TestCase("123A Main St Seattle WA", "123", "A")]
        [TestCase("123-A Main St Seattle WA", "123", "A")]
        [TestCase("123-1/2 Main St Seattle WA", "123", "1/2")]
        public void AllKinds(
            string text,
            string number,
            string fraction)
        {
            var address = Test(text);
            Assert.AreEqual(number, address.Street.Number);
            Assert.AreEqual(fraction, address.Street.NumberFraction);
        }
    }
}
