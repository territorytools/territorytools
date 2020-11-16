using NUnit.Framework;
using System;
using System.Collections.Generic;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class ParserTests
    {
        const string STREET_TYPES = "KEY,ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

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

        [TestCase("123A Main St Seattle WA", "123", "A")]
        [TestCase("123-A Main St Seattle WA", "123", "A")]
        [TestCase("123-1/2 Main St Seattle WA", "123", "1/2")]
        public void Parse_StreetNumberFractions(
            string text, 
            string number, 
            string fraction)
        {
            AssertStreetNumberFraction(text, number, fraction);
        }

        void AssertStreetNumberFraction(string text, string number, string fraction)
        {
            var address = Test(text);
            Assert.AreEqual(number, address.Street.Number);
            Assert.AreEqual(fraction, address.Street.NumberFraction);
        }

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

        [TestCase("POB 321", "321", "POB")]
        [TestCase("PO Box 321", "321", "PO Box")]
        [TestCase("P O B 321", "321", "P O B")]
        [TestCase("P.O.Box 321", "321", "P.O.Box")]
        [TestCase("P.O. Box 321", "321", "P.O. Box")]
        [TestCase("P. O. B. 321", "321", "P. O. B.")]
        [TestCase("P. O. Box 321", "321", "P. O. Box")]
        [TestCase("Post Office Box 321", "321", "Post Office Box")]
        public void Parse_NonStreet_PO_Box(string text, string number, string name)
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
        public void Parse_NonStreet_PO_Box_WithCity(
            string text, 
            string number, 
            string name, 
            string city, 
            string region)
        {
            AssertNonStreetNumberName(text, number, name, city, region);
        }

        [Test]
        public void Parse_NonStreet_Lot_Number()
        {
            AssertNonStreetNumberName("Lot 321", "321", "Lot");
        }

        [Test]
        public void Parse_NonStreet_Post_Office_Barn_Fails()
        {
            AssertNonStreetNumberName("Post Office Barn 321", string.Empty, string.Empty);
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
        public void Parse_Unit_Normal(
            string text, 
            string unitType, 
            string number)
        {
            AssertUnitTypeNumber(text, unitType, number);
        }

        [TestCase("123 N Main St Lynnwood WA 98087", "N")]        
        [TestCase("123 S Main St Lynnwood WA 98087", "S")]
        [TestCase("123 W Main St Lynnwood WA 98087","W")]
        [TestCase("123 E Main St Lynnwood WA 98087", "E")]
        [TestCase("123 SW Main St Lynnwood WA 98087", "SW")]
        [TestCase("123 SE Main St Lynnwood WA 98087", "SE")]
        [TestCase("123 NE Main St Lynnwood WA 98087", "NE")]
        [TestCase("123 NW Main St Lynnwood WA 98087", "NW")]
        [TestCase("123 North Main St Lynnwood WA 98087", "North")]
        [TestCase("123 South Main St Lynnwood WA 98087", "South")]
        [TestCase("123 East Main St Lynnwood WA 98087", "East")]
        [TestCase("123 West Main St Lynnwood WA 98087", "West")]
        [TestCase("123 Northeast Main St Lynnwood WA 98087", "Northeast")]
        [TestCase("123 Northwest Main St Lynnwood WA 98087", "Northwest")]
        [TestCase("123 Southeast Main St Lynnwood WA 98087", "Southeast")]
        [TestCase("123 Southwest Main St Lynnwood WA 98087", "Southwest")]
        public void Parse_DirectionalPrefix_Normal(
            string text, 
            string directionalPrefix)
        {
            Assert.AreEqual(directionalPrefix, Test(text).Street.Name.DirectionalPrefix);
        }

        [TestCase("123 Main St N Lynnwood WA 98087", "N")]
        [TestCase("123 Main St S Lynnwood WA 98087", "S")]
        [TestCase("123 Main St W Lynnwood WA 98087", "W")]
        [TestCase("123 Main St E Lynnwood WA 98087", "E")]
        [TestCase("123 Main St SW Lynnwood WA 98087", "SW")]
        [TestCase("123 Main St SE Lynnwood WA 98087", "SE")]
        [TestCase("123 Main St NE Lynnwood WA 98087", "NE")]
        [TestCase("123 Main St NW Lynnwood WA 98087", "NW")]
        [TestCase("123 Main St North Lynnwood WA 98087", "North")]
        [TestCase("123 Main St South Lynnwood WA 98087", "South")]
        [TestCase("123 Main St East Lynnwood WA 98087", "East")]
        [TestCase("123 Main St West Lynnwood WA 98087", "West")]
        [TestCase("123 Main St Northeast Lynnwood WA 98087", "Northeast")]
        [TestCase("123 Main St Northwest Lynnwood WA 98087", "Northwest")]
        [TestCase("123 Main St Southeast Lynnwood WA 98087", "Southeast")]
        [TestCase("123 Main St Southwest Lynnwood WA 98087", "Southwest")]
        public void Parse_DirectionalSuffix_Normal(string text, string directionalSuffx)
        {
            Assert.AreEqual(directionalSuffx, Test(text).Street.Name.DirectionalSuffix);
        }

        [Test]
        public void Parse_StreetType_StreetOnly_Simple()
        {
            Assert.AreEqual("St", Test("123 Main St").Street.Name.StreetType);
        }

        [Test]
        public void Parse_StreetType_WithCityAndRegion()
        {
            Assert.AreEqual("St", Test("123 Main St Lynnwood WA").Street.Name.StreetType);
        }

        [Test]
        public void Parse_StreetType_WithCityEtc()
        {
            Assert.AreEqual("St", Test("123 Main St Lynnwood WA 98087").Street.Name.StreetType);
        }

        [Test]
        public void Parse_StreetType_WithDirectionalPrefixAndCityEtc()
        {
            Assert.AreEqual("St", Test("123 NE Main St Lynnwood WA 98087").Street.Name.StreetType);
        }

        [Test]
        public void Parse_StreetType_WithDirectionalSuffixAndCityEtc()
        {
            Assert.AreEqual("St", Test("123 Main St NE Lynnwood WA 98087").Street.Name.StreetType);
        }

        [Test]
        public void Parse_StreetType_WithUnitAndCityEtc()
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
        public void Parse_Unit_JustTheNumber(string text, string unitType, string unitNumber)
        {
            AssertUnitTypeNumber(text, unitType, unitNumber);
        }

        [TestCase("123 Main St NE A Lynnwood WA 98087", "", "A")]
        public void Unit_JustTheNumber_AfterDirectional(string text, string unitType, string unitNumber)
        {
            AssertUnitTypeNumber(text, unitType, unitNumber);
        }

        [Test]
        public void Parse_StreetType_StreetAndUnitOnly()
        {
            Assert.AreEqual("Unit", Test("123 Main St Unit # 5-A").Unit.Type);
            Assert.AreEqual("5-A", Test("123 Main St Unit # 5-A").Unit.Number);
            Assert.AreEqual("St", Test("123 Main St Unit # 5-A").Street.Name.StreetType);
        }

        [Test]
        public void Parse_PrefixStreetType_WithCityEtc()
        {
            Assert.AreEqual("Hwy", Test("123 Hwy 456 Lynnwood WA 98087").Street.Name.StreetTypePrefix);
            Assert.AreEqual("456", Test("123 Hwy 456 Lynnwood WA 98087").Street.Name.Name);
        }

        [Test]
        public void Parse_PrefixStreetType_WithUnitCityEtc()
        {
            Assert.AreEqual("Hwy", Test("123 Hwy 456 Unit 5A Lynnwood WA 98087").Street.Name.StreetTypePrefix);
            Assert.AreEqual("456", Test("123 Hwy 456 Unit 5A Lynnwood WA 98087").Street.Name.Name);
        }

        
        [TestCase("123 Hwy 456 Unit 5A Lynnwood WA 98123", "123", "456", "")]
        [TestCase("1234 North Rd Lynnwood WA 98123", "1234", "North", "Rd")]
        public void Parse_NorthRdEtc(string text, string streetNumber, string streetName, string streetType)
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
        public void Parse_NorthRd()
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
        public void Parse_DirectionalName_WithUnitCityEtc()
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
        public void Parse_DirectionalName2_WithUnitCityEtc()
        {
            AssertParts(
                text: "1234 West Blue Mountain Rd SE Seattle WA 98123",
                streetNumber: "1234",
                dirPrefix: "",
                streetName: "West Blue Mountain",
                streetType: "Rd",
                dirSuffix: "SE",
                city: "Seattle",
                region: "WA",
                postal: "98123");
        }

        [Test]
        public void Parse_UnRecognizedCity_ReturnsLastWord()
        {
            string text = "12345 SE Pl Nowhere WA";
            Assert.AreEqual("Nowhere", Test(text).City.Name);
        }

        [Test]
        public void Parse_MissingStreetName_WithUnitCityEtc()
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
        //10823 NE, , Kirkland, WA
        [Test]
        public void Parse_MissingStreetName_WithDirPrefix()
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
        public void Parse_Commas_4()
        {
            string text = "123 Main St, Unit # 5-A, Lynnwood, WA, 98087";
            var address = Parse(text);
            Assert.AreEqual("123", address.Street.Number);
            Assert.AreEqual("Main", address.Street.Name.Name);
            Assert.AreEqual("St", address.Street.Name.StreetType);
            Assert.AreEqual("Unit", address.Unit.Type);
            Assert.AreEqual("5-A", address.Unit.Number);
            Assert.AreEqual("Lynnwood", address.City.Name);
            Assert.AreEqual("WA", address.Region.Code);
            Assert.AreEqual("98087", address.Postal.Code);
        }

        //[TestCase("123 Hwy 456 Unit 5A Lynnwood WA 98123", "123", "456")]
        //[TestCase("1234 North Rd Lynnwood WA 98123", "1234", "North", "Rd")]
        public void AssertParts(
            string text,
            string streetNumber,
            string dirPrefix,
            string streetName,
            string streetType,
            string dirSuffix,
            string city,
            string region,
            string postal)
        {
            var address = Parse(text);
            Assert.AreEqual(streetNumber, address.Street.Number, "Street.Number");
            Assert.AreEqual(dirPrefix, address.Street.Name.DirectionalPrefix, "Street.Name.DirectionalPrefix)");
            Assert.AreEqual(streetName, address.Street.Name.Name, "Street.Name.Name");
            Assert.AreEqual(streetType, address.Street.Name.StreetType, "Street.Name.StreetType");
            Assert.AreEqual(dirSuffix, address.Street.Name.DirectionalSuffix, "Street.Name.DirectionalSuffix");
            Assert.AreEqual(city, address.City.Name, "City.Name");
            Assert.AreEqual(region, address.Region.Code, "Region.Code");
            Assert.AreEqual(postal, address.Postal.Code, "Postal.Code");
        }

        Address Test(string text)
        {
            return Parse(text);
        }

        private static Address Parse(string text)
        {
            var parser = new Parser(
                new List<string> {
                    "Seattle",
                    "Sammamish",
                    "Everett",
                    "Lynnwood",
                    "Bend",
                    "North Bend",
                    "Lake Forest Park" },
                StreetType.Split(STREET_TYPES),
                StreetType.Split(StreetType.PrefixDefaults));

            return parser.Parse(text);
        }

        void AssertStreetNumberName(string text, string streetNumber, string streetName)
        {
            var address = Test(text);
            Assert.AreEqual(streetName, address.Street.Name.Name.ToString());
            Assert.AreEqual(streetNumber, address.Street.Number.ToString());
        }

        void AssertNonStreetNumberName(
            string text, 
            string streetNumber, 
            string streetName, 
            string city = null, 
            string region = null)
        {
            var address = Test(text);
            Assert.IsEmpty(address.Street.Name.Name.ToString());
            Assert.AreEqual(streetName, address.Street.Name.NamePrefix.ToString());
            Assert.AreEqual(streetNumber, address.Street.Number.ToString());
            if(city != null)
                Assert.AreEqual(city, address.City.Name.ToString());
            if (region != null)
                Assert.AreEqual(region, address.Region.Code.ToString());
        }

        void AssertUnitTypeNumber(string text, string type, string number)
        {
            var address = Test(text);
            Assert.AreEqual(type, address.Unit.Type.ToString());
            Assert.AreEqual(number, address.Unit.Number.ToString());
        }

        void AssertCityRegion(string text, string city, string region)
        {
            var address = Test(text);
            Assert.AreEqual(city, address.City.Name);
            Assert.AreEqual(region, address.Region.Code);
        }
    }
}
