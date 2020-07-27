using AlbaConsole;
using CommandLine;
using Controllers.AlbaServer;
using Controllers.ExtraFileFormats;
using Controllers.S13;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerritoryTools.Alba.Cli.Verbs;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;

namespace TerritoryTools.Alba.Cli
{
    class Program
    {
        static List<string> Arguments;
        const string StreetTypes = "ALLEY:ALY,ANNEX:ANX,ARCADE:ARC,AVENUE:AVE,BAYOO:BYU,BEACH:BCH,BEND:BND,BLUFF:BLF,BLUFFS:BLFS,BOTTOM:BTM,BOULEVARD:BLVD,BRANCH:BR,BRIDGE:BRG,BROOK:BRK,BROOKS:BRKS,BURG:BG,BURGS:BGS,BYPASS:BYP,CAMP:CP,CANYON:CYN,CAPE:CPE,CAUSEWAY:CSWY,CENTER:CTR,CENTERS:CTRS,CIRCLE:CIR,CIRCLES:CIRS,CLIFF:CLF,CLIFFS:CLFS,CLUB:CLB,COMMON:CMN,CORNER:COR,CORNERS:CORS,COURSE:CRSE,COURT:CT,COURTS:CTS,COVE:CV,COVES:CVS,CREEK:CRK,CRESCENT:CRES,CREST:CRST,CROSSING:XING,CROSSROAD:XRD,CURVE:CURV,DALE:DL,DAM:DM,DIVIDE:DV,DRIVE:DR,DRIVES:DRS,ESTATE:EST,ESTATES:ESTS,EXPRESSWAY:EXPY,EXTENSION:EXT,EXTENSIONS:EXTS,FALL:FALL,FALLS:FLS,FERRY:FRY,FIELD:FLD,FIELDS:FLDS,FLAT:FLT,FLATS:FLTS,FORD:FRD,FORDS:FRDS,FOREST:FRST,FORGE:FRG,FORGES:FRGS,FORK:FRK,FORKS:FRKS,FORT:FT,FREEWAY:FWY,GARDEN:GDN,GARDENS:GDNS,GATEWAY:GTWY,GLEN:GLN,GLENS:GLNS,GREEN:GRN,GREENS:GRNS,GROVE:GRV,GROVES:GRVS,HARBOR:HBR,HARBORS:HBRS,HAVEN:HVN,HEIGHTS:HTS,HIGHWAY:HWY,HILL:HL,HILLS:HLS,HOLLOW:HOLW,INLET:INLT,INTERSTATE:I,ISLAND:IS,ISLANDS:ISS,ISLE:ISLE,JUNCTION:JCT,JUNCTIONS:JCTS,KEY:KY,KEYS:KYS,KNOLL:KNL,KNOLLS:KNLS,LAKE:LK,LAKES:LKS,LAND:LAND,LANDING:LNDG,LANE:LN,LIGHT:LGT,LIGHTS:LGTS,LOAF:LF,LOCK:LCK,LOCKS:LCKS,LODGE:LDG,LOOP:LOOP,MALL:MALL,MANOR:MNR,MANORS:MNRS,MEADOW:MDW,MEADOWS:MDWS,MEWS:MEWS,MILL:ML,MILLS:MLS,MISSION:MSN,MOORHEAD:MHD,MOTORWAY:MTWY,MOUNT:MT,MOUNTAIN:MTN,MOUNTAINS:MTNS,NECK:NCK,ORCHARD:ORCH,OVAL:OVAL,OVERPASS:OPAS,PARK:PARK,PARKS:PARK,PARKWAY:PKWY,PARKWAYS:PKWY,PASS:PASS,PASSAGE:PSGE,PATH:PATH,PIKE:PIKE,PINE:PNE,PINES:PNES,PLACE:PL,PLAIN:PLN,PLAINS:PLNS,PLAZA:PLZ,POINT:PT,POINTS:PTS,PORT:PRT,PORTS:PRTS,PRAIRIE:PR,RADIAL:RADL,RAMP:RAMP,RANCH:RNCH,RAPID:RPD,RAPIDS:RPDS,REST:RST,RIDGE:RDG,RIDGES:RDGS,RIVER:RIV,ROAD:RD,ROADS:RDS,ROUTE:RTE,ROW:ROW,RUE:RUE,RUN:RUN,SHOAL:SHL,SHOALS:SHLS,SHORE:SHR,SHORES:SHRS,SKYWAY:SKWY,SPRING:SPG,SPRINGS:SPGS,SPUR:SPUR,SPURS:SPUR,SQUARE:SQ,SQUARES:SQS,STATION:STA,STREAM:STRM,STREET:ST,STREETS:STS,SUMMIT:SMT,TERRACE:TER,THROUGHWAY:TRWY,TRACE:TRCE,TRACK:TRAK,TRAIL:TRL,TUNNEL:TUNL,TURNPIKE:TPKE,UNDERPASS:UPAS,UNION:UN,UNIONS:UNS,VALLEY:VLY,VALLEYS:VLYS,VIADUCT:VIA,VIEW:VW,VIEWS:VWS,VILLAGE:VLG,VILLAGES:VLGS,VILLE:VL,VISTA:VIS,WALK:WALK,WALKS:WALK,WALL:WALL,WAY:WAY,WAYS:WAYS,WELL:WL,WELLS:WLS";

        static void Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<
                    DownloadAddressesOptions,
                    DownloadAssignmentsOptions>
                    (args)
                   .MapResult(
                     (DownloadAddressesOptions opts) => opts.Run(),
                     (DownloadAssignmentsOptions opts) => opts.Run(),
                     errs => 1);

                /*
                if (args.Length < 1)
                {
                    PrintUsage();
                    return;
                }

                Arguments = args.ToList();
                
                if (string.Equals(Arguments[0], "debug", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Press any key to continue");
                    Console.WriteLine("Pausing so you can attach a debugger...");
                    Console.ReadKey();
                    Arguments.RemoveAt(0);
                }

                if (Arguments.Count < 1)
                {
                    PrintUsage();
                    return;
                }

                string command = Arguments[0];

                if (string.Equals(command, "show-credentials"))
                {
                    Console.WriteLine("Account: " + Environment.GetEnvironmentVariable("alba_account"));
                    Console.WriteLine("user: " + Environment.GetEnvironmentVariable("alba_user"));
                    string password = Environment.GetEnvironmentVariable("alba_password");
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("password: MISSING");
                    }
                    else
                    {
                        Console.WriteLine("password: HIDDEN");
                    }
                }
                else if (string.Equals(command, "download-addresses"))
                {
                    DownloadAddresses(Arguments);
                }
                else if (string.Equals(command, "download-assignments"))
                {
                    DownloadAssignments(Arguments);
                }
                else if (string.Equals(command, "count-addresses"))
                {
                    CountAddresses(Arguments);
                }
                else if (string.Equals(command, "normalize-addresses"))
                {
                    NormalizeAddresses(Arguments);
                }
                else if (string.Equals(command, "convert-addresses"))
                {
                    ConvertLetterWritingAddresses(Arguments);
                }
                else if (string.Equals(command, "match-addresses"))
                {
                    MatchAddresses(Arguments);
                }
                else if (string.Equals(command, "exclude-addresses"))
                {
                    ExcludeAddresses(Arguments);
                }
                else if (string.Equals(command, "sort-addresses"))
                {
                    SortAddresses(Arguments);
                }
                else if (string.Equals(command, "list-languages"))
                {
                    ListLanguages(Arguments);
                }
                else if (string.Equals(command, "filter-language"))
                {
                    FilterLanguage(Arguments);
                }
                else if (string.Equals(command, "filter-status"))
                {
                    FilterStatus(Arguments);
                }
                else if (string.Equals(command, "pivot-s-13"))
                {
                    PivotS13(Arguments);
                }
                else if (string.Equals(command, "last-completed"))
                {
                    LastCompleted(Arguments);
                }
                else if (string.Equals(command, "take"))
                {
                    Take(Arguments);
                }
                */
            }
            catch (NormalException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e);
            }
        }

        public static AuthorizationClient AlbaClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "www.alba-website-here.com",
                applicationPath: "/alba");

            var client = new AuthorizationClient(
                webClient: webClient,
                basePath: basePath);

            return client;
        }

        static void PrintUsage()
        {
            Console.WriteLine("Alba Client Console");
            Console.WriteLine("Usage: alba.exe <command> <parameters>");
            Console.WriteLine("Command: show-credentials");
            Console.WriteLine("Command: download-addresses <alba-csv-output-file>");
            Console.WriteLine("Command: download-assignments <alba-csv-output-file>");
            // TODO: Download borders as KML command
            // TODO: Download users command
            // TODO: Upload commands
            Console.WriteLine("Command: filter-language <language1,language2...> <alba-csv-input-file> <alba-csv-output-file>");
            Console.WriteLine("Command: list-languages <alba-tsv-input-file> ");
            Console.WriteLine("Command: filter-status <status> <alba-csv-input-file> <alba-csv-output-file>");
            Console.WriteLine("Command: sort-addresses <alba-tsv-input-file> <alba-tsv-output-file>");
            Console.WriteLine("Command: count-addresses <alba-tsv-input-file>");
            Console.WriteLine("Command: convert-addresses <alba-tsv-input-file> <alba-tsv-output-file>");
            Console.WriteLine("Command: normalize-addresses <alba-tsv-input-file> <alba-tsv-output-file>");
            Console.WriteLine("Command: match-addresses <alba-tsv-input-file> <alba-tsv-match-file> <alba-tsv-output-file>");
            Console.WriteLine("Command: exclude-addresses <alba-tsv-input-file> <alba-tsv-exclusion-file> <alba-tsv-output-file>");
            Console.WriteLine("Command: take <number-of-lines> <input-file>");
            Console.WriteLine("For download commands you must set credentials as environment variables:");
            Console.WriteLine("  Variables: alba_account, alba_user, alba_password");
            Console.WriteLine("  Windows: set alba_account=mygroup");
            Console.WriteLine("  PowerShell: $Env:alba_account=\"mygroup\"");
            Console.WriteLine("  Linux/macOS: export alba_account=mygroup");
        }
       
        static void FilterLanguage(List<string> args)
        {
            Console.WriteLine("Filtering Languages");
            if (args.Count < 4)
            {
                throw new NormalException("Not enough arguments!  Usage: alba filter-language '<language1,language2...>' <alba-tsv-input-file> <alba-tsv-output-file>");
            }

            string languages = args[1];
            string inputPath = args[2];
            string outputPath = args[3];


            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");
            Console.WriteLine($"Languages: {languages}");
            Console.WriteLine($"Language List:");
            var languageList = languages.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var normalizedLanguages = new List<string>();
            foreach (string lang in languageList)
            {
                Console.WriteLine($"  {lang.Trim()}");
                normalizedLanguages.Add(lang.Trim().ToUpper());
            }

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            Console.WriteLine($"Before Count: {addresses.Count()}");

            var filtered = addresses
                .Where(a => normalizedLanguages.Contains(a.Language.ToUpper()))
                //.Where(a => a.Language.StartsWith("Chinese"))
                .ToList();

            Console.WriteLine($"After Filter Count: {filtered.Count}");
            
            LoadTsvAlbaAddresses.SaveTo(filtered, outputPath);
        }

        static void FilterStatus(List<string> args)
        {
            Console.WriteLine("Filtering Status");
            if (args.Count < 4)
            {
                throw new NormalException("Not enough arguments!  Usage: alba filter-status status <alba-tsv-input-file> <alba-tsv-output-file>");
            }

            string status = args[1];
            string inputPath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Target Status: {status}");
            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            Console.WriteLine($"Before Count: {addresses.Count()}");

            var filtered = addresses
                .Where(a => string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine($"After Filter Count: {filtered.Count}");

            LoadTsvAlbaAddresses.SaveTo(filtered, outputPath);
        }

        static void PivotS13(List<string> args)
        {
            Console.WriteLine("Pivoting S-13 form...");
            if (args.Count != 4)
            {
                throw new NormalException("Wrong number of arguments!  Usage: alba pivot-s-13 <s-13-csv-input-file> <alba-assignments-csv-input-file> <s-13-csv-output-file>");
            }

            string inputPath = args[1];
            string albaTerritoryAssignmentsPath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Alba Assignment File Path: {albaTerritoryAssignmentsPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            Console.WriteLine("Loading...");
            var rows = PivotAssignmentRowsToS13Columns.LoadFrom(inputPath);

            Console.WriteLine("Removing entries with Checked-In and Checked-Out both blank...");
            var cleaned = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.CheckedIn)
                    || !string.IsNullOrWhiteSpace(r.CheckedOut))
                .ToList();

            Console.WriteLine("Pivoting...");
            var columns = PivotAssignmentRowsToS13Columns.PivotFrom(cleaned);

            Console.WriteLine("Adding unworked territories...");
            var assignments = DownloadTerritoryAssignments.LoadFromCsv(albaTerritoryAssignmentsPath);
            var errors = new List<string>();
            foreach (var assignment in assignments)
            {
                try
                {
                    int assignmentNumber = int.Parse(assignment.Number);

                    if (!columns.Exists(c => string.Equals(c.Territory, assignmentNumber.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var newCol = new S13Column
                        {
                            Territory = assignment.Number
                        };

                        newCol.Entries.Add(new S13Entry { Publisher = "Never Worked" });
                        columns.Add(newCol);
                    }
                }
                catch(Exception e)
                {
                    errors.Add($"Number: {assignment.Number}: {e.Message}");
                }
            }

            foreach(string error in errors)
            {
                Console.WriteLine(error);
            }

            var orderedColumns = columns.OrderBy(c => int.Parse(c.Territory)).ToList();

            Console.WriteLine("Saving data to new file...");
            PivotAssignmentRowsToS13Columns.SaveTo(orderedColumns, outputPath);
        }

        static void LastCompleted(List<string> args)
        {
            Console.WriteLine("Generating 'last completed' from S-13 form...");
            if (args.Count != 4)
            {
                throw new NormalException("Wrong number of arguments!  Usage: alba last-completed <s-13-csv-input-file> <alba-assignments-csv-input-file> <s-13-csv-output-file>");
            }

            string inputPath = args[1];
            string albaTerritoryAssignmentsPath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Alba Assignment File Path: {albaTerritoryAssignmentsPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            Console.WriteLine("Loading...");
            var rows = PivotAssignmentRowsToS13Columns.LoadFrom(inputPath);

            Console.WriteLine("Removing entries with Checked-In blank...");
            var cleaned = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.CheckedIn))
                .ToList();

            var parsed = PivotAssignmentRowsToS13Columns.LastCompletedFrom(cleaned);

            Console.WriteLine("Adding unworked territories...");
            var albaAssignments = DownloadTerritoryAssignments.LoadFromCsv(albaTerritoryAssignmentsPath);
            var errors = new List<string>();
            foreach (var assignment in albaAssignments)
            {
                try
                {
                    int number = int.Parse(assignment.Number);

                    if (!parsed.Exists(c => string.Equals(c.Territory, number.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var t = new TerritoryLastCompleted
                        {
                            Territory = assignment.Number,
                            TimesWorked = 0,
                            Publisher = "Never Completed",
                        };

                        parsed.Add(t);
                    }
                }
                catch (Exception e)
                {
                    errors.Add($"Number: {assignment.Number}: {e.Message}");
                }
            }

            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }

            Console.WriteLine("Sorting by number...");
            var ordered = parsed.OrderBy(c => int.Parse(c.Territory)).ToList();

            Console.WriteLine("Saving data to new file...");
            PivotAssignmentRowsToS13Columns.SaveTo(ordered, outputPath);
        }

        static void CountAddresses(List<string> args)
        {
            Console.WriteLine("Count Addresses");
            if (args.Count < 2)
            {
                throw new NormalException("Not enough arguments!  Usage: alba count-addresses <alba-tsv-input-file>");
            }

            string inputPath = args[1];

            Console.WriteLine($"Input File Path: {inputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            Console.WriteLine($"Address Count: {addresses.Count()}");
        }

        static void Take(List<string> args)
        {
            Console.WriteLine("Take a number of lines (excluding header)");
            if (args.Count < 4)
            {
                throw new NormalException("Not enough arguments!  Usage: alba take <number-of-lines> <alba-tsv-input-file> <output-file>");
            }

            int lines = int.Parse(args[1]);
            string inputPath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Number of lines to take: {lines}");
            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var f = File.OpenText(inputPath);
            var o = File.CreateText(outputPath);
            for(int i = 0; i <= lines; i++) 
            {
                o.WriteLine(f.ReadLine());
            }

            o.Flush();
            o.Close();
            f.Close();
        }

        static void NormalizeAddresses(List<string> args)
        {
            Console.WriteLine("Normalize Addresses");
            if (args.Count < 3)
            {
                throw new NormalException("Not enough arguments!  Usage: alba normalize-addresses <alba-tsv-input-file> <alba-tsv-output-file>");
            }

            string inputPath = args[1];
            string outputPath = args[2];

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
                catch(Exception e)
                {
                    errors.Add(a);
                    Console.WriteLine(e.Message);
                }
            }

            if (errors.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Errors:");
                foreach(var a in errors)
                {
                    Console.WriteLine(a.Address);
                }

                Console.WriteLine($"Count: {errors.Count}");
            }

            LoadTsvAlbaAddresses.SaveTo(normalized, outputPath);
            LoadTsvAlbaAddresses.SaveTo(errors, $"{outputPath}.errors.txt");
        }

        static void ConvertLetterWritingAddresses(List<string> args)
        {
            Console.WriteLine("Convert Letter Writing Addresses");
            if (args.Count < 3)
            {
                throw new NormalException("Not enough arguments!  Usage: alba covert-addresses <letter-writing-csv-input-file> <alba-tsv-output-file>");
            }

            string inputPath = args[1];
            string outputPath = args[2];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var writingAddresses = LoadCsv<LetterWritingSheet>.LoadFrom(inputPath);
            var albaAddresses = new List<AlbaAddressExport>();
            var errors = new List<LetterWritingSheet>();

            foreach (var a in writingAddresses)
            {
                try
                {
                    albaAddresses.Add(
                        new AlbaAddressExport
                        {
                            Address = a.Address,
                            Suite = a.Suite,
                            City = a.City,
                            Postal_code = a.ZIP,
                            Province = a.State,
                        }
                    );
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

            LoadTsvAlbaAddresses.SaveTo(albaAddresses, outputPath);
            LoadCsv<LetterWritingSheet>.SaveTo(errors, $"{outputPath}.errors.txt");
        }

        static void SortAddresses(List<string> args)
        {
            Console.WriteLine("Sort Addresses");
            if (args.Count < 3)
            {
                throw new NormalException("Not enough arguments!  Usage: alba sort-addresses <alba-tsv-input-file> <alba-tsv-output-file>");
            }

            string inputPath = args[1];
            string outputPath = args[2];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);
            var errors = new List<AlbaAddressExport>();
            var output = new List<AlbaAddressExport>();
            var sorted = addresses
                .OrderBy(a => a.City)
                .ThenBy(a => a.Address)
                .ThenBy(a => a.Suite)
                .ToList();

            LoadTsvAlbaAddresses.SaveTo(sorted, outputPath);
        }

        static void MatchAddresses(List<string> args)
        {
            Console.WriteLine("Match Addresses");
            if (args.Count < 3)
            {
                throw new NormalException("Not enough arguments!  Usage: alba match-addresses <alba-tsv-input-file> <csv-match-file> <alba-tsv-output-file>");
            }

            string inputPath = args[1];
            string matchPath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"Match File Path: {matchPath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);
            var matches = LoadTsvAlbaAddresses.LoadFrom(matchPath);

            var errors = new List<AlbaAddressExport>();
            var output = new List<AlbaAddressExport>();

            var streetTypes = StreetType.Parse(StreetTypes);
            var parser = new CompleteAddressParser(streetTypes);

            foreach (var a in addresses)
            {
                try
                {
                    foreach (var b in matches)
                    {
                        //string aText = $"{a.Address}, {a.Suite}, {a.City}, {a.Province} {a.Postal_code}";
                        //string bText = $"{b.Address}, {b.Suite}, {b.City}, {b.Province} {b.Postal_code}";
                        //if (parser.Parse(aText)
                        //    .SameAs(
                        //        other: parser.Parse(bText), 
                        //        options: Address.SameAsOptions.ComparePostalCode))
                        //{ 
                        if (string.Equals(a.Address, b.Address, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(a.Suite, b.Suite, StringComparison.OrdinalIgnoreCase)
                            && (string.Equals(a.City, b.City, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(a.Postal_code, b.Postal_code, StringComparison.OrdinalIgnoreCase))
                            && string.Equals(a.Province, b.Province, StringComparison.OrdinalIgnoreCase))
                        {
                            output.Add(a);
                        }
                    }
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

            LoadTsvAlbaAddresses.SaveTo(output, outputPath);

            if (errors.Count > 0)
            {
                LoadTsvAlbaAddresses.SaveTo(errors, $"{outputPath}.errors.txt");
            }
        }


        static void ExcludeAddresses(List<string> args)
        {
            Console.WriteLine("Exclude Addresses");
            if (args.Count < 3)
            {
                throw new NormalException("Not enough arguments!  Usage: alba exclude-addresses <alba-tsv-input-file> <alba-tsv-to-exclude-file> <alba-tsv-output-file>");
            }

            string inputPath = args[1];
            string toExcludePath = args[2];
            string outputPath = args[3];

            Console.WriteLine($"Input File Path: {inputPath}");
            Console.WriteLine($"To Exclude File Path: {toExcludePath}");
            Console.WriteLine($"Output File Path: {outputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);
            var exclusions = LoadTsvAlbaAddresses.LoadFrom(toExcludePath);

            var errors = new List<AlbaAddressExport>();
            var output = new List<AlbaAddressExport>();

            foreach (var address in addresses)
            {
                try
                {
                    bool wasMatched = false;
                    foreach (var exclusion in exclusions)
                    {
                        if (string.Equals(address.Address, exclusion.Address, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(address.Suite, exclusion.Suite, StringComparison.OrdinalIgnoreCase)
                            && (string.Equals(address.City, exclusion.City, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(address.Postal_code, exclusion.Postal_code, StringComparison.OrdinalIgnoreCase))
                            && string.Equals(address.Province, exclusion.Province, StringComparison.OrdinalIgnoreCase))
                        {
                            wasMatched = true;
                            break;
                        }
                    }

                    if (!wasMatched)
                    {
                        output.Add(address);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(address);
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

            LoadTsvAlbaAddresses.SaveTo(output, outputPath);

            if (errors.Count > 0)
            {
                LoadTsvAlbaAddresses.SaveTo(errors, $"{outputPath}.errors.txt");
            }
        }
        
        static void ListLanguages(List<string> args)
        {
            Console.WriteLine("List Languages from Alba TSV Address File");
            if (args.Count < 2)
            {
                throw new NormalException("Not enough arguments!  Usage: alba list-languages <alba-tsv-input-file>");
            }

            string inputPath = args[1];

            Console.WriteLine($"Input File Path: {inputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(inputPath);

            var langGroups = addresses
                .GroupBy(a => a.Language);

            Console.WriteLine("Languages:");
            foreach(var group in langGroups.OrderBy(g => g.Key))
            {
                Console.WriteLine($"{group.Key}: {group.Count()}");
            }

            Console.WriteLine($"Count: {langGroups.Count()}");
        }

        public static Credentials GetCredentials()
        {
            string account = Environment.GetEnvironmentVariable("alba_account");
            string user = Environment.GetEnvironmentVariable("alba_user");
            string password = Environment.GetEnvironmentVariable("alba_password");

            if (string.IsNullOrWhiteSpace(account)
                || string.IsNullOrWhiteSpace(user)
                || string.IsNullOrWhiteSpace(password))
            {
                throw new NormalException("Missing credentials, please set your credentials as environment variables:  alba_account, alba_user, alba_password");
            }

            return new Credentials(
                account,
                user,
                password);
        }
    }
}
