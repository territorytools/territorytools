using NUnit.Framework;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class StreetTypeTests
    {
        [TestCase("St", "Street", true)]
        [TestCase("Street", "St", true)]
        [TestCase("St", "Rd", false)]
        [TestCase("Street", "Road", false)]
        [TestCase("St", "Road", false)]
        [TestCase("Street", "Rd", false)]
        [TestCase("", "", true)]
        [TestCase("", null, true)]
        [TestCase(null, "", true)]
        public void SameSuffixTest(string first, string second, bool expected)
        {
            Assert.AreEqual(expected, StreetType.SameSuffix(first, second));
        }

        [TestCase("Street", "St")]
        [TestCase("Road", "Rd")]
        [TestCase("ST", "St")]
        [TestCase("STREET", "St")]
        [TestCase("X", "X")]
        [TestCase("x", "X")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void NormalizeTest(string streetType, string expected)
        {
            Assert.AreEqual(expected, StreetType.Normalize(streetType));
        }
    }
}

