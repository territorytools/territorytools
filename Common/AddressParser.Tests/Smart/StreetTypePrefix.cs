using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    class StreetTypePrefix : ParserTestBase
    {
        [TestCase("POB 321", "321", "POB")]
        [TestCase("PO Box 321", "321", "PO Box")]
        [TestCase("P O B 321", "321", "P O B")]
        [TestCase("P.O.Box 321", "321", "P.O.Box")]
        [TestCase("P.O. Box 321", "321", "P.O. Box")]
        [TestCase("P. O. B. 321", "321", "P. O. B.")]
        [TestCase("P. O. Box 321", "321", "P. O. Box")]
        [TestCase("Post Office Box 321", "321", "Post Office Box")]
        [TestCase("Lot 321", "321", "Lot")]
        public void NonStreet(string text, string number, string name)
        {
            AssertNonStreetNumberName(text, number, name);
        }

        [TestCase("POB 321, Bellevue, WA 98004", "321", "POB", "Bellevue", "WA")]
        [TestCase("PO Box 321, Bellevue, WA 98004", "321", "PO Box", "Bellevue", "WA")]
        [TestCase("P O B 321, Bellevue, WA 98004", "321", "P O B", "Bellevue", "WA")]
        [TestCase("P.O.Box 321, Bellevue, WA 98004", "321", "P.O.Box", "Bellevue", "WA")]
        [TestCase("P.O. Box 321, Bellevue, WA 98004", "321", "P.O. Box", "Bellevue", "WA")]
        [TestCase("P. O. B. 321, Bellevue, WA 98004", "321", "P. O. B.", "Bellevue", "WA")]
        [TestCase("P. O. Box 321, Bellevue, WA 98004", "321", "P. O. Box", "Bellevue", "WA")]
        [TestCase("Post Office Box 321, Bellevue, WA 98004", "321", "Post Office Box", "Bellevue", "WA")]
        public void NonStreet_WithCityRegion(
            string text,
            string number,
            string name,
            string city,
            string region)
        {
            AssertNonStreetNumberName(text, number, name, city, region);
        }

        [Test]
        public void PrefixStreetType_WithCityEtc()
        {
            var address = Test("123 Hwy 456 Lynnwood WA 98087");
            Assert.AreEqual("Hwy", address.Street.Name.StreetTypePrefix);
            Assert.AreEqual("456", address.Street.Name.Name);
        }

        [Test]
        public void PrefixStreetType_WithUnitCityEtc()
        {
            string text = "123 Hwy 456 Unit 5A Lynnwood WA 98087";
            Assert.AreEqual("Hwy", Test(text).Street.Name.StreetTypePrefix);
            Assert.AreEqual("456", Test(text).Street.Name.Name);
        }

        [TestCase("123 Hwy 456 Lynnwood WA 98087", "Hwy", "456")]
        [TestCase("123 Hwy 456 Unit 5A Lynnwood WA 98087", "Hwy", "456")]
        public void PrefixStreetType_Tests(string text, string streetTypePrefix, string streetName)
        {
            Assert.AreEqual("Hwy", Test(text).Street.Name.StreetTypePrefix);
            Assert.AreEqual("456", Test(text).Street.Name.Name);
        }

        [TestCase("123 Hwy 456 # 5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 # 5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 # 5-A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 # A5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 # A-5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 # A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit 5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit 5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit 5-A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit A5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit A-5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #5-A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #A5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #A-5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit #A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # 5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # 5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # 5-A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # A5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # A-5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 Unit # A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #5-A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #A5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #A-5 Lynnwood WA 98123", "123", "456", "")]
        [TestCase("123 Hwy 456 #A Lynnwood WA 98123", "123", "456", "")]
        public void WithUnitTypeAndNumber(string text, string streetNumber, string streetName, string streetType)
        {
            AssertParts(
                text: text,
                streetNumber: streetNumber,
                dirPrefix: "",
                streetName: streetName,
                streetType: streetType,
                dirSuffix: "",
                city: "Lynnwood",
                region: "WA",
                postal: "98123");
        }
    }
}
