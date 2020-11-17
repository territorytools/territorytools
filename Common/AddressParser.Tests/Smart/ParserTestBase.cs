using NUnit.Framework;
using System.Collections.Generic;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class ParserTestBase
    {
        const string STREET_TYPES = "KEY,ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        public Address Test(string text)
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
                StreetType.Map(STREET_TYPES),
                StreetType.Split(StreetType.PrefixDefaults));

            return parser.Parse(text);
        }

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

        public void AssertStreetNumberName(string text, string streetNumber, string streetName)
        {
            var address = Test(text);
            Assert.AreEqual(streetName, address.Street.Name.Name.ToString());
            Assert.AreEqual(streetNumber, address.Street.Number.ToString());
        }

        public void AssertNonStreetNumberName(
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
            if (city != null)
                Assert.AreEqual(city, address.City.Name.ToString());
            if (region != null)
                Assert.AreEqual(region, address.Region.Code.ToString());
        }

        public void AssertUnitTypeNumber(string text, string type, string number)
        {
            var address = Test(text);
            Assert.AreEqual(type, address.Unit.Type.ToString());
            Assert.AreEqual(number, address.Unit.Number.ToString());
        }

        public void AssertCityRegion(string text, string city, string region)
        {
            var address = Test(text);
            Assert.AreEqual(city, address.City.Name);
            Assert.AreEqual(region, address.Region.Code);
        }
    }
}
