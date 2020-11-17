using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class DirectionalStreetNames : ParserTestBase
    {
       

        [Test]
        public void OneWord_NoSuffix()
        {
            AssertParts(
                text: "1234 North Rd Sammamish WA 98123",
                streetNumber: "1234",
                dirPrefix: "",
                streetName: "North",
                streetType: "Rd",
                dirSuffix: "",
                city: "Sammamish",
                region: "WA",
                postal: "98123");
        }

        [Test]
        public void ThreeWords_WithSuffix()
        {
            AssertParts(
                text: "1234 W Lake Sammamish Pkwy SE Seattle WA 98123",
                streetNumber: "1234",
                dirPrefix: "",
                streetName: "W Lake Sammamish",
                streetType: "Pkwy",
                dirSuffix: "SE",
                city: "Seattle",
                region: "WA",
                postal: "98123");
        }

        [Test]
        public void MissingStreetNameAndType_WithDirPrefix()
        {
            string text = "12345 NE Lynnwood WA";
            Assert.AreEqual("12345", Test(text).Street.Number);
            Assert.IsEmpty(Test(text).Street.Name.DirectionalPrefix);
            Assert.AreEqual("NE", Test(text).Street.Name.Name);
            Assert.IsEmpty(Test(text).Street.Name.StreetType);
            Assert.AreEqual("Lynnwood", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [Test]
        public void MissingStreetName_WithDirectionalAndStreetType()
        {
            string text = "12345 SE Pl Lynnwood WA";
            Assert.AreEqual("12345", Test(text).Street.Number);
            Assert.AreEqual("", Test(text).Street.Name.DirectionalPrefix);
            Assert.AreEqual("SE", Test(text).Street.Name.Name);
            Assert.AreEqual("Pl", Test(text).Street.Name.StreetType);
            Assert.AreEqual("", Test(text).Street.Name.DirectionalSuffix);
            Assert.AreEqual("Lynnwood", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [TestCase("1234 North Rd Lynnwood WA 98123", "1234", "North", "Rd")]
        public void DirectionalStreetName(string text, string streetNumber, string streetName, string streetType)
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

        [Test]
        public void UnknownCityName_ReturnsLastWord()
        {
            string text = "12345 SE Pl Nowhere WA";
            Assert.AreEqual("Nowhere", Test(text).City.Name);
        }
    }
}
