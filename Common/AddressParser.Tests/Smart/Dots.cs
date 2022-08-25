using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class Dots : ParserTestBase
    {
        [TestCase("123 Main Street Ste. 234 Seattle WA", "St")]
        [TestCase("123 Main St. Seattle WA", "St")]
        [TestCase("123 Main St. Ste. 234 Seattle WA", "St")]
        [TestCase("123 N. Main St. Ste. 234 Seattle WA", "St")]
        public void StreetType(string text, string type)
        {
            var address = Normalize(text);
            Assert.AreEqual(type, address.Street.Name.StreetType);
        }
        
        [TestCase("123 Main St N. Seattle WA", "", "N")]
        [TestCase("123 Main St. N. Seattle WA", "", "N")]
        [TestCase("123 N. Main St. Ste. 234 Seattle WA", "N", "")]
        [TestCase("123 Main St. N. Ste. 234 Seattle WA", "", "N")]
        public void Directionals(string text, string prefix, string suffix)
        {
            var address = Normalize(text);
            Assert.AreEqual(prefix, address.Street.Name.DirectionalPrefix);
            Assert.AreEqual(suffix, address.Street.Name.DirectionalSuffix);
        }

        [TestCase("123 Mr. Main St N Seattle WA", "MR MAIN")]
        [TestCase("123 N. Main St. SE Ste. 234 Seattle WA", "N MAIN")]
        [TestCase("123 Main St. N. Ste. 234 Seattle WA", "MAIN")]
        public void StreetName(string text, string name)
        {
            var address = Normalize(text);
            Assert.AreEqual(name, address.Street.Name.Name);
        }

        [TestCase("123 Main St Seattle Wa.", "WA")]
        [TestCase("123 Main St Seattle Wa. 98123", "WA")]
        public void Region(string text, string region)
        {
            var address = Normalize(text);
            Assert.AreEqual(region, address.Region.Code);
        }

        [TestCase("123 Hwy. 99 Seattle WA", "HWY")]
        [TestCase("123 Hwy. 99 N Seattle WA", "HWY")]
        public void StreetTypePrefix(string text, string prefix)
        {
            var address = Normalize(text);
            Assert.AreEqual(prefix, address.Street.Name.StreetTypePrefix);
        }
    }
}
