using NUnit.Framework;
using System.Collections.Generic;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_City_Name_Null()
        {
            Assert.IsNull(Test(null).City.Name);
        }

        [Test]
        public void Parse_Street_Number_StreetOnly()
        {
            Assert.AreEqual("123", Test("123 Main St").Street.Number.ToString());
        }

        [Test]
        public void Parse_Street_Name_StreetOnly()
        {
            Assert.AreEqual("Main St", Test("123 Main St").Street.Name.ToString());
        }

        //[Test]
        //public void Parse_Commas_StreetCityRegion()
        //{
        //    string text = "123 Main St, Seattle, WA";
        //    Assert.AreEqual("Main St", Test(text).Street.Name.ToString());
        //    Assert.AreEqual("Seattle", Test(text).City.Name);
        //    Assert.AreEqual("WA", Test(text).Region.Code);
        //}

        [Test]
        public void Parse_Street_TypelessName_wCityAndRegion()
        {
            string text = "123 Broadway Everett WA";
            Assert.AreEqual("Broadway", Test(text).Street.Name.ToString());
            Assert.AreEqual("Everett", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [Test]
        public void Parse_Street_TypelessName_TwoWordCityAndRegion()
        {
            string text = "123 Main North Bend WA";
            Assert.AreEqual("Main", Test(text).Street.Name.ToString());
            Assert.AreEqual("North Bend", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [Test]
        public void Parse_Street_TypelessName_OneWordCityAndRegion()
        {
            string text = "123 Main South Bend WA";
            Assert.AreEqual("Main South", Test(text).Street.Name.ToString());
            Assert.AreEqual("Bend", Test(text).City.Name);
            Assert.AreEqual("WA", Test(text).Region.Code);
        }

        [Test]
        public void Parse_NonStreet_PO_Box()
        {
            AssertStreetNumberName("POB 321", "321", "POB");
            AssertStreetNumberName("PO Box 321", "321", "PO Box");
            AssertStreetNumberName("P O B 321", "321", "P O B");
            AssertStreetNumberName("P.O.Box 321", "321", "P.O.Box");
            AssertStreetNumberName("P.O. Box 321", "321", "P.O. Box");
            AssertStreetNumberName("P. O. B. 321", "321", "P. O. B.");
            AssertStreetNumberName("P. O. Box 321", "321", "P. O. Box");
            AssertStreetNumberName("Post Office Box 321", "321", "Post Office Box");
        }

        [Test]
        public void Parse_NonStreet_Lot_Number()
        {
            AssertStreetNumberName("Lot 321", "321", "Lot");
        }

        [Test]
        public void Parse_NonStreet_Post_Office_Barn_Fails()
        {
            AssertStreetNumberName("Post Office Barn 321", string.Empty, string.Empty);
        }

        [Test]
        public void Parse_Region_Code_Normal()
        {
            Assert.AreEqual("WA", Test("123 Main St Lynnwood WA 98087").Region.Code.ToString());
        }

        [Test]
        public void Parse_Postal_Code_Normal()
        {
            Assert.AreEqual("98087", Test("123 Main St Lynnwood WA 98087").Postal.Code.ToString());
        }

        [Test]
        public void Parse_Unit_Normal()
        {
            AssertUnitTypeNumber("123 Main St Apt 234 Lynnwood WA 98087", "Apt", "234");
            AssertUnitTypeNumber("123 Main St Apt 1A Lynnwood WA 98087", "Apt", "1A");
            AssertUnitTypeNumber("123 Main St Apt 23-34 Lynnwood WA 98087", "Apt", "23-34");
            AssertUnitTypeNumber("123 Main St Apt 23-A Lynnwood WA 98087", "Apt", "23-A");
            AssertUnitTypeNumber("123 Main St Apt A Lynnwood WA 98087", "Apt", "A");
            AssertUnitTypeNumber("123 Main St Unit A Lynnwood WA 98087", "Unit", "A");
            AssertUnitTypeNumber("123 Main St Unit AA Lynnwood WA 98087", "Unit", "AA");
            AssertUnitTypeNumber("123 Main St Unit # 234 Lynnwood WA 98087", "Unit", "234");
            AssertUnitTypeNumber("123 Main St Unit #234 Lynnwood WA 98087", "Unit", "234");
            AssertUnitTypeNumber("123 Main St Unit #234A Lynnwood WA 98087", "Unit", "234A");
            AssertUnitTypeNumber("123 Main St Unit #234-A Lynnwood WA 98087", "Unit", "234-A");
            AssertUnitTypeNumber("123 Main St #234 Lynnwood WA 98087", "#", "234");
            AssertUnitTypeNumber("123 Main St #234A Lynnwood WA 98087", "#", "234A");
            AssertUnitTypeNumber("123 Main St #234-A Lynnwood WA 98087", "#", "234-A");
            AssertUnitTypeNumber("123 Main St #A Lynnwood WA 98087", "#", "A");
            AssertUnitTypeNumber("123 Main St # A Lynnwood WA 98087", "#", "A");
            AssertUnitTypeNumber("123 Main St # 234 Lynnwood WA 98087", "#", "234");
            AssertUnitTypeNumber("123 Main St # 234A Lynnwood WA 98087", "#", "234A");
            AssertUnitTypeNumber("123 Main St # 234-A Lynnwood WA 98087", "#", "234-A");
            AssertUnitTypeNumber("123 Main St # 234-34 Lynnwood WA 98087", "#", "234-34");
            AssertUnitTypeNumber("123 Broadway #A Everett WA 98087", "#", "A");
        }

        [Test]
        public void Parse_DirectionalPrefix_Normal()
        {
            Assert.AreEqual("N", Test("123 N Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("S", Test("123 S Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("W", Test("123 W Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("E", Test("123 E Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("SW", Test("123 SW Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("SE", Test("123 SE Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("NE", Test("123 NE Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("NW", Test("123 NW Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("North", Test("123 North Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("South", Test("123 South Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("East", Test("123 East Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("West", Test("123 West Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("Northeast", Test("123 Northeast Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("Northwest", Test("123 Northwest Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("Southeast", Test("123 Southeast Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
            Assert.AreEqual("Southwest", Test("123 Southwest Main St Lynnwood WA 98087").Street.Name.DirectionalPrefix);
        }

        [Test]
        public void Parse_DirectionalSuffix_Normal()
        {
            Assert.AreEqual("N", Test("123 Main St N Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("S", Test("123 Main St S Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("W", Test("123 Main St W Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("E", Test("123 Main St E Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("SW", Test("123 Main St SW Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("SE", Test("123 Main St SE Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("NE", Test("123 Main St NE Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("NW", Test("123 Main St NW Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("North", Test("123 Main St North Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("South", Test("123 Main St South Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("East", Test("123 Main St East Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("West", Test("123 Main St West Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("Northeast", Test("123 Main St Northeast Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("Northwest", Test("123 Main St Northwest Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("Southeast", Test("123 Main St Southeast Lynnwood WA 98087").Street.Name.DirectionalSuffix);
            Assert.AreEqual("Southwest", Test("123 Main St Southwest Lynnwood WA 98087").Street.Name.DirectionalSuffix);
        }

        Address Test(string text)
        {
            var parser = new Parser(
                new List<string> { 
                    "Seattle",
                    "Everett",
                    "Lynnwood",
                    "Bend",
                    "North Bend", 
                    "Lake Forest Park" });

            return parser.Parse(text);
        }

        void AssertStreetNumberName(string text, string streetNumber, string streetName)
        {
            Assert.AreEqual(streetName, Test(text).Street.Name.Name.ToString());
            Assert.AreEqual(streetNumber, Test(text).Street.Number.ToString());
        }

        void AssertUnitTypeNumber(string text, string type, string number)
        {
            Assert.AreEqual(type, Test(text).Unit.Type.ToString());
            Assert.AreEqual(number, Test(text).Unit.Number.ToString());
        }
    }
}
