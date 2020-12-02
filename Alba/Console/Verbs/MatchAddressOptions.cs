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
        "match-addresses",
        HelpText = "Match addresses from a two Alba TSV files")]
    public class MatchAddressOptions
    {
        [Option(
            "input-path",
            Required = true,
            HelpText = "Input file of addresses in Alba TSV format")]
        [Value(0)]
        public string InputPath { get; set; }

        [Option(
            "match-path",
            Required = true,
            HelpText = "Path of address file to match with input file in Alba TSV format")]
        [Value(0)]
        public string MatchPath { get; set; }

        [Option(
            "output-path",
            Required = true,
            HelpText = "Input file of addresses in Alba TSV format")]
        [Value(0)]
        public string OutputPath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Matc addresses example",
                        new MatchAddressOptions {
                            InputPath = "from-alba.tsv",
                            MatchPath = "matching.tsv",
                            OutputPath = "for-alba.tsv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Match Addresses");
            Console.WriteLine($"Input File Path: {InputPath}");
            Console.WriteLine($"Match File Path: {MatchPath}");
            Console.WriteLine($"Output File Path: {OutputPath}");

            var addresses = LoadTsvAlbaAddresses.LoadFrom(InputPath);
            var matches = LoadTsvAlbaAddresses.LoadFrom(MatchPath);

            var errors = new List<AlbaAddressExport>();
            var output = new List<AlbaAddressExport>();

            var streetTypes = StreetType.Parse(NormalizeAddressesOptions.StreetTypes);
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

            LoadTsvAlbaAddresses.SaveTo(output, OutputPath);

            if (errors.Count > 0)
            {
                LoadTsvAlbaAddresses.SaveTo(errors, $"{OutputPath}.errors.txt");
            }

            return 0;
        }

    }
}
