using NUnit.Framework;
using System;
using System.Collections.Generic;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class FullAddressParserTests : ParserTestBase
    {
        const string STREET_TYPES = "KEY,ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        [Test]
        public void Empty_CityName()
        {
            Assert.IsEmpty(ParseIgnoreCityRegion(null).City.Name);
        }

        [Test]
        public void StreetNumber()
        {
            Assert.AreEqual("123", ParseIgnoreCityRegion("123 Main St").Street.Number.ToString());
        }

        [Test]
        public void StreetName()
        {
            var address = ParseIgnoreCityRegion("123 Main St");
            Assert.AreEqual("Main St", address.Street.Name.ToString());
        }

        [Test]
        public void NonStreet_PartialMatch()
        {
            // Starts with same letters, but ends wrong
            var address = ParseIgnoreCityRegion("Post Office Barn 321");
            Assert.IsEmpty(address.Street.Name.NamePrefix);
        }

        [Test]
        public void Region_Code_Normal()
        {
            Assert.AreEqual("WA", Test("123 Main St Lynnwood WA 98087").Region.Code.ToString());
        }

        [Test]
        public void Postal_Code_Normal()
        {
            Assert.AreEqual("98087", Test("123 Main St Lynnwood WA 98087").Postal.Code.ToString());
        }

        [Test]
        public void StreetType_StreetOnly_Simple()
        {
            Assert.AreEqual("St", ParseIgnoreCityRegion("123 Main St").Street.Name.StreetType);
        }

        [Test]
        public void StreetType_WithCityAndRegion()
        {
            Assert.AreEqual("St", Test("123 Main St Lynnwood WA").Street.Name.StreetType);
        }

        [Test]
        public void StreetType_WithCityRegionPostal()
        {
            Assert.AreEqual("St", Test("123 Main St Lynnwood WA 98087").Street.Name.StreetType);
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

        Address Test(string text)
        {
            return Parse(text);
        }

        private static Address ParseIgnoreCityRegion(string text)
        {
            Parser parser = DefaultParser();
            parser.IgnoreMissingCity = true;
            parser.IgnoreMissingRegion = true;

            return parser.Parse(text);
        }

        private static Address Parse(string text)
        {
            Parser parser = DefaultParser();

            return parser.Parse(text);
        }

        private static Parser DefaultParser()
        {
            var validRegions = new List<string> { "WA", "CA", "OR", "ID" };
            return new Parser(
                validRegions,
                new List<string> {
                    "Seattle",
                    "Sammamish",
                    "Everett",
                    "Lynnwood",
                    "Bend",
                    "North Bend",
                    "Lake Forest Park" },
                StreetType.Split(STREET_TYPES),
                StreetType.Map(STREET_TYPES),
                StreetType.Split(StreetType.PrefixDefaults));
        }
    }
}
