using CommandLine;
using CommandLine.Text;
using Controllers.AlbaServer;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "normalize-addresses",
        HelpText = "Normalize addresses from a Alba TSV")]
    public class NormalizeAddressesOptions
    {
        public const string StreetTypes = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        [Option(
            "input-path",
            Required = true,
            HelpText = "Input file of addresses in Alba TSV format")]
        [Value(0)]
        public string inputPath { get; set; }

        [Option(
            "output-path",
            Required = true,
            HelpText = "Output file for addresses to upload to Alba (TSV format)")]
        [Value(0)]
        public string outputPath { get; set; }


        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Normalize addresses  example",
                        new NormalizeAddressesOptions {
                            inputPath = "from-alba.tsv",
                            outputPath = "for-alba.tsv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Normalize Addresses");            
            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            var errors = new List<AlbaAddressExport>();
            var normalized = new List<AlbaAddressExport>();

            foreach (var a in addresses)
            {
                try
                {
                    //string before = $"{a.Address}, {a.Suite}, {a.City}, {a.Province}, {a.Postal_code}";
                    var address = parser.Normalize($"{a.Address}, {a.Suite}", a.City, a.Province, a.Postal_code);
                    a.Address = address.CombineStreet();
                    a.Suite = address.CombineUnit();
                    a.City = address.City;
                    a.Province = address.State;
                    a.Postal_code = address.PostalCode;
                    normalized.Add(a);
                    //Console.WriteLine($"{before} :: {a.Address}, {a.Suite}, {a.City}, {a.Province}, {a.Postal_code}");
                }
                catch (Exception e)
                {
                    errors.Add(a);
                    Console.WriteLine(e.Message);
                }
            }

            if (errors.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Errors:");
                foreach (var a in errors)
                {
                    Console.WriteLine(a.Address);
                }

                Console.WriteLine($"Count: {errors.Count}");
            }

            LoadTsvAlbaAddresses.SaveTo(normalized, outputPath);
            LoadTsvAlbaAddresses.SaveTo(errors, $"{outputPath}.errors.txt");

            return 0;
        }
    }
}
