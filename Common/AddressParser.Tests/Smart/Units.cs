using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class Units : ParserTestBase
    {
        [TestCase("123 Main St Apt 234 Lynnwood WA 98087", "Apt", "234")]
        [TestCase("123 Main St Apt 1A Lynnwood WA 98087", "Apt", "1A")]
        [TestCase("123 Main St Apt 23-34 Lynnwood WA 98087", "Apt", "23-34")]
        [TestCase("123 Main St Apt 23-A Lynnwood WA 98087", "Apt", "23-A")]
        [TestCase("123 Main St Apt A Lynnwood WA 98087", "Apt", "A")]
        [TestCase("123 Main St Unit A Lynnwood WA 98087", "Unit", "A")]
        [TestCase("123 Main St Unit AA Lynnwood WA 98087", "Unit", "AA")]
        [TestCase("123 Main St Unit # 234 Lynnwood WA 98087", "Unit", "234")]
        [TestCase("123 Main St Unit #234 Lynnwood WA 98087", "Unit", "234")]
        [TestCase("123 Main St Unit #234A Lynnwood WA 98087", "Unit", "234A")]
        [TestCase("123 Main St Unit #234-A Lynnwood WA 98087", "Unit", "234-A")]
        [TestCase("123 Main St #234 Lynnwood WA 98087", "#", "234")]
        [TestCase("123 Main St #234A Lynnwood WA 98087", "#", "234A")]
        [TestCase("123 Main St #234-A Lynnwood WA 98087", "#", "234-A")]
        [TestCase("123 Main St #A Lynnwood WA 98087", "#", "A")]
        [TestCase("123 Main St # A Lynnwood WA 98087", "#", "A")]
        [TestCase("123 Main St # 234 Lynnwood WA 98087", "#", "234")]
        [TestCase("123 Main St # 234A Lynnwood WA 98087", "#", "234A")]
        [TestCase("123 Main St # 234-A Lynnwood WA 98087", "#", "234-A")]
        [TestCase("123 Main St # 234-34 Lynnwood WA 98087", "#", "234-34")]
        [TestCase("123 Broadway #A Everett WA 98087", "#", "A")]
        public void Unit_Normal(
            string text,
            string unitType,
            string number)
        {
            AssertUnitTypeNumber(text, unitType, number);
        }

        [Test]
        public void StreetType_WithUnitAndCityEtc()
        {
            Assert.AreEqual("Unit", Test("123 Main St Unit # 5-A Lynnwood WA 98087").Unit.Type);
            Assert.AreEqual("5-A", Test("123 Main St Unit # 5-A Lynnwood WA 98087").Unit.Number);
            Assert.AreEqual("St", Test("123 Main St Unit # 5-A Lynnwood WA 98087").Street.Name.StreetType);
        }

        [TestCase("123 Main St 5 Lynnwood WA 98087", "", "5")]
        [TestCase("123 Main St A Lynnwood WA 98087", "", "A")]
        [TestCase("123 Main St 5-A Lynnwood WA 98087", "", "5-A")]
        [TestCase("123 Main St A-5 Lynnwood WA 98087", "", "A-5")]
        [TestCase("123 Main St A-B Lynnwood WA 98087", "", "A-B")]
        [TestCase("123 Main St 567 Lynnwood WA 98087", "", "567")]
        [TestCase("123 Main St 567-89 Lynnwood WA 98087", "", "567-89")]
        [TestCase("123 Main St 5A Lynnwood WA 98087", "", "5A")]
        [TestCase("123 Main St A5 Lynnwood WA 98087", "", "A5")]
        public void Unit_JustTheNumber(string text, string unitType, string unitNumber)
        {
            AssertUnitTypeNumber(text, unitType, unitNumber);
        }

        [TestCase("123 Main St NE A Lynnwood WA 98087", "", "A")]
        public void Unit_JustTheNumber_AfterDirectional(string text, string unitType, string unitNumber)
        {
            AssertUnitTypeNumber(text, unitType, unitNumber);
        }

        [TestCase("123 Main St NE, A, Lynnwood WA 98087", "", "A")]
        public void Unit_WithComma_AfterDirectional(string text, string unitType, string unitNumber)
        {
            AssertUnitTypeNumber(text, unitType, unitNumber);
        }

        [Test]
        public void StreetType_StreetAndUnitOnly()
        {
            string text = "123 Main St Unit # 5-A";
            Assert.AreEqual("Unit", Test(text).Unit.Type);
            Assert.AreEqual("5-A", Test(text).Unit.Number);
            Assert.AreEqual("St", Test(text).Street.Name.StreetType);
        }

        [Test]
        public void StreetType_DoubleUnitType()
        {
            string text = "123 Main St ##5-A";
            Assert.AreEqual("#", Test(text).Unit.Type);
            Assert.AreEqual("5-A", Test(text).Unit.Number);
            Assert.AreEqual("St", Test(text).Street.Name.StreetType);
        }
    }
}
