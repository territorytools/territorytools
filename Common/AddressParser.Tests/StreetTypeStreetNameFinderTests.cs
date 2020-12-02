using NUnit.Framework;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;

namespace TerritoryEntities.Tests.AddressParsers
{
    [TestFixture]
    public class StreetTypeStreetNameFinderTests
    {
        public const string StreetTypes = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";


        [Test]
        public void Normalize_NoStreetType_Full()
        {
            // Arrange
            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            var address = parser.Normalize(@"1234 Broadway, Everett, WA 12345");

            Assert.AreEqual("Broadway", address.StreetName);
            Assert.AreEqual("1234", address.Number);
            Assert.IsTrue(address.IsNotPhysical);
        }

        [Test]
        public void Normalize_NoStreetType()
        {
            // Arrange
            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            var address = parser.Normalize(@"1234 Broadway", "Everett", "WA", "12345");

            Assert.AreEqual("Broadway", address.StreetName);
            Assert.AreEqual("1234", address.Number);
            Assert.IsTrue(address.IsNotPhysical);
        }

        [Test]
        public void Normalize_POBox_Test()
        {
            // Arrange
            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            var address = parser.Normalize(@"P.O. Box 1234", "Bellevue", "WA", "12345");
  
            Assert.AreEqual("PO Box", address.StreetName);
            Assert.AreEqual("1234", address.Number);
            Assert.IsTrue(address.IsNotPhysical);
        }

        [Test]
        public void Find_Broadway_E_Is_A_StreetNameWithNoStreetType()
        {
            // Arrange
            var container = new AddressParseContainer(@"523 Broadway E Apt 664, Seattle WA 98102-5381");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Broadway", container.ParsedAddress.StreetName.Value);
        }

        private static StreetTypeStreetNameFinder GetFinder(AddressParseContainer container)
        {
            var streetTypes = StreetType.From(
                new string[]
                {
                    "ALLEY",
                    "AVENUE",
                    "CIRCLE",
                    "WAY"
                });

            var splitter = new AddressSplitter(container);
            var finder = new StreetTypeStreetNameFinder(container, streetTypes);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
