using CommandLine;
using CommandLine.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using Controllers.AlbaServer;
using Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
       "filter-addresses",
       HelpText = "Filter addresses out into a Alba CSV file")]
    public class FilterAddressOptions : IOptionsWithRun
    {
        [Option(
           "source-addresses",
           Required = true,
           HelpText = "Input file path from source Alba address export (tab separated)")]
        [Value(0)]
        public string SourceAddresses { get; set; }

        [Option(
          "notes-private-contain",
          Required = false,
          HelpText = "Find records where Notes_private contain this value")]
        [Value(0)]
        public string NotesPrivateContain { get; set; }

        [Option(
          "postal-code",
          Required = false,
          HelpText = "Find records where Postal_code equal this value")]
        [Value(0)]
        public string PostalCode { get; set; }

        [Option(
         "alba-territory-id",
         Required = false,
         HelpText = "Find records with the Alba territory id")]
        [Value(0)]
        public int AlbaTerritoryId { get; set; }

        [Option(
            "output",
            Required = true,
            HelpText = "Output remain addresses in Alba file path (Alba CSV)")]
        [Value(0)]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Add phone numbers example",
                        new FilterAddressOptions {
                            SourceAddresses = "addresses.csv",
                            NotesPrivateContain = "addresses-with-notes.csv",
                            OutputFilePath = "output-for-alba.csv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Removing addresses by id...");
            Console.WriteLine($"SourceAddresses: {SourceAddresses}");
            Console.WriteLine($"NotesPrivateContain: {NotesPrivateContain}");
            Console.WriteLine($"AlbaTerritoryId: {AlbaTerritoryId}");
            Console.WriteLine($"OutputFilePath: {OutputFilePath}");

            var source = LoadCsv<AlbaAddressExport>.LoadFrom(SourceAddresses);

            Console.WriteLine($"Source Address Count: {source.Count()}");

            if (!string.IsNullOrWhiteSpace(NotesPrivateContain))
            {
                var filtered = source
                    .Where(a => a.Notes_private.Contains(NotesPrivateContain))
                    .ToList();

                Console.WriteLine($"Filtered Addresses: {filtered.Count}");

                var results = new List<AlbaAddressImport>();
                foreach (var address in filtered)
                {
                    results.Add(AlbaAddressImport.From(address));
                }

                Console.WriteLine($"Result Addresses: {results.Count}");

                LoadCsvAddresses.SaveTo(results, OutputFilePath);
            }
            else if (!string.IsNullOrWhiteSpace(PostalCode))
            {
                var filtered = source
                    .Where(a => a.Postal_code.Equals(PostalCode))
                    .ToList();

                Console.WriteLine($"Filtered Addresses: {filtered.Count}");

                var results = new List<AlbaAddressImport>();
                foreach (var address in filtered)
                {
                    results.Add(AlbaAddressImport.From(address));
                }

                Console.WriteLine($"Result Addresses: {results.Count}");

                LoadCsvAddresses.SaveTo(results, OutputFilePath);
            }
            else if(AlbaTerritoryId != 0)
            {
                var filtered = source
                    .Where(a => a.Territory_ID == AlbaTerritoryId)
                    .ToList();

                Console.WriteLine($"Filtered Addresses: {filtered.Count}");

                var results = new List<AlbaAddressImport>();
                foreach (var address in filtered)
                {
                    results.Add(AlbaAddressImport.From(address));
                }

                Console.WriteLine($"Result Addresses: {results.Count}");

                LoadCsvAddresses.SaveTo(results, OutputFilePath);
            }
            else
            {
                Console.WriteLine("No filters where set");
            }

            return 0;
        }
    }
}
