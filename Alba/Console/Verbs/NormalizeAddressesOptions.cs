using CommandLine;
using CommandLine.Text;
using Controllers.AlbaServer;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using smart = TerritoryTools.Common.AddressParser.Smart;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;
using System.Linq;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "normalize-addresses",
        HelpText = "Normalize addresses from a Alba TSV")]
    public class NormalizeAddressesOptions : IOptionsWithRun
    {
        public const string StreetTypes = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPEEDWAY:SPEEDWAY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        [Option(
            "input-path",
            Required = false,
            HelpText = "Input file of addresses in Alba TSV format")]
        [Value(0)]
        public string inputPath { get; set; }
        
        [Option(
           "input-address-only",
           Required = false,
           HelpText = "Input file of just addresses, no units, cities, postal codes, etc, in a text file")]
        [Value(0)]
        public string inputAddressOnly { get; set; }

        [Option(
            "output-path",
            Required = true,
            HelpText = "Output file for addresses to upload to Alba (TSV format)")]
        [Value(0)]
        public string outputPath { get; set; }

        [Option(
           "cities",
           Required = false,
           HelpText = "List of comma separate cities.  If omitted, cities are")]
        public string Cities { get; set; }

        [Option(
            "use-old-parser",
            Required = true,
            HelpText = "Use old parser engine")]
        public bool UseOldParser { get; set; }

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

            if(UseOldParser)
                return OldParser();
            
            return NewParser();
        }

        int NewParser()
        {
            Console.WriteLine("Using new parser");
            if (string.IsNullOrWhiteSpace(inputPath) && string.IsNullOrWhiteSpace(inputAddressOnly))
            {
                throw new ArgumentException("input-path and input-address-only cannot both be missing.");
            }

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Input Address Only Text File Path: {inputAddressOnly}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            Console.WriteLine("Cities:");
            var cities = addresses
                .Where(a=>!StreetTypes.Contains(a.City.ToUpper()))
                .OrderBy(a=>a.City)
                .Select(a => a.City)
                .Distinct()
                .ToList();

            foreach(string city in cities)
            {
                Console.WriteLine($"  {city}");
            }

            var validRegions = smart.Region.Split(smart.Region.Defaults);
            var streetTypes = smart.StreetType.Split(smart.StreetType.Defaults);
            var mapStreetTypes = smart.StreetType.Map(smart.StreetType.Defaults);
            var prefixStreetTypes = smart.StreetType.Split(smart.StreetType.PrefixDefaults);
            var parser = new smart.Parser(validRegions, cities, streetTypes, mapStreetTypes, prefixStreetTypes);

            //var streetTypes = StreetType.Parse(StreetTypes);
            //var parser = new CompleteAddressParser(streetTypes);

            var errors = new List<AlbaAddressExport>();
            var normalized = new List<AlbaAddressExport>();
            var errorLogBuilder = new StringBuilder(10_000);

            if (!string.IsNullOrWhiteSpace(inputPath))
            {
                foreach (var a in addresses)
                {
                    try
                    {
                        //var address = parser.Parse(null, a.Address, a.Suite, a.City, a.Province, a.Postal_code);
                        var address = parser.Parse($"{a.Address}, {a.Suite}, {a.City}, {a.Province} {a.Postal_code}");
                        a.Address = address.Street.ToString().Trim();
                        a.Suite = address.Unit.ToString();
                        a.City = address.City.ToString();
                        a.Province = address.Region.ToString();
                        a.Postal_code = address.Postal.Code?.ToString();
                        normalized.Add(a);
                    }
                    catch (Exception e)
                    {
                        errors.Add(a);
                        errorLogBuilder.AppendLine(e.Message);
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

                string errorFilePath = outputPath;
                if (Regex.IsMatch(outputPath, @"(.*)(\..+)"))
                {
                    errorFilePath = Regex.Replace(outputPath, @"(.*)(\..+)", "$1");
                }

                LoadTsvAlbaAddresses.SaveTo(errors, $"{errorFilePath}.errors.txt");
                System.IO.File.WriteAllText($"{errorFilePath}.errors.log", errorLogBuilder.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(inputAddressOnly))
            {
                var builder = new StringBuilder(1_000_000);
                builder.AppendLine("Address");

                var addressList = System.IO.File.ReadAllLines(inputAddressOnly);
                foreach (string a in addressList)
                {
                    try
                    {
                        Console.WriteLine("Normalizing: " + a);
                        //.Parse(null, a.Address, a.Suite, a.City, a.Province, a.Postal_code);
                        var address = parser.Parse(null, a, null, "No City", "XX", "99999");
                        builder.AppendLine(address.ToString());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error processing address: {a}");
                    }
                }

                System.IO.File.WriteAllText(outputPath, builder.ToString());
            }

            return 0;
        }

        int OldParser()
        {
            Console.WriteLine("Using old parser");
            if (string.IsNullOrWhiteSpace(inputPath) && string.IsNullOrWhiteSpace(inputAddressOnly))
            {
                throw new ArgumentException("input-path and input-address-only cannot both be missing.");
            }

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Input Address Only Text File Path: {inputAddressOnly}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            var errors = new List<AlbaAddressExport>();
            var normalized = new List<AlbaAddressExport>();
            var errorLogBuilder = new StringBuilder(10_000);

            if (!string.IsNullOrWhiteSpace(inputPath))
            {
                foreach (var a in addresses)
                {
                    try
                    {
                        var address = parser.Normalize($"{a.Address}, {a.Suite}", a.City, a.Province, a.Postal_code);
                        a.Address = address.CombineStreet();
                        a.Suite = address.CombineUnit();
                        a.City = address.City;
                        a.Province = address.State;
                        a.Postal_code = address.PostalCode;
                        normalized.Add(a);
                    }
                    catch (Exception e)
                    {
                        errors.Add(a);
                        errorLogBuilder.AppendLine(e.Message);
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

                string errorFilePath = outputPath;
                if (Regex.IsMatch(outputPath, @"(.*)(\..+)"))
                {
                    errorFilePath = Regex.Replace(outputPath, @"(.*)(\..+)", "$1");
                }

                LoadTsvAlbaAddresses.SaveTo(errors, $"{errorFilePath}.errors.txt");
                System.IO.File.WriteAllText($"{errorFilePath}.errors.log", errorLogBuilder.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(inputAddressOnly))
            {
                var builder = new StringBuilder(1_000_000);
                builder.AppendLine("Address");

                var addressList = System.IO.File.ReadAllLines(inputAddressOnly);
                foreach (string a in addressList)
                {
                    try
                    {
                        Console.WriteLine("Normalizing: " + a);
                        var address = parser.Normalize(a, "No City", "XX", "99999");
                        builder.AppendLine(address.CombineStreet());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error processing address: {a}");
                    }
                }

                System.IO.File.WriteAllText(outputPath, builder.ToString());
            }

            return 0;
        }
    }
}
