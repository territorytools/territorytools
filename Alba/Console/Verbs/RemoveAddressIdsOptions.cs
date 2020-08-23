using CommandLine;
using CommandLine.Text;
using Controllers.AlbaServer;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.ListServices;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
       "remove-address-ids",
       HelpText = "Find missing Address_ID in Alba tab separated export file")]
    public class RemoveAddressIdsOptions
    {
        [Option(
           "source-addresses",
           Required = true,
           HelpText = "Input file path from source Alba address export (tab separated)")]
        [Value(0)]
        public string SourceAddresses { get; set; }

        [Option(
          "remove-addresses",
          Required = true,
          HelpText = "Input file path from removed Alba address CSV")]
        [Value(0)]
        public string RemoveAddresses { get; set; }

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
                        new AddPhoneNumbersOptions {
                            PhoneNumbers = "phone-numbers.csv",
                            Addresses = "addresses.csv",
                            OutputFilePath = "for-alba.csv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Removing addresses by id...");
            Console.WriteLine($"SourceAddresses: {SourceAddresses}");
            Console.WriteLine($"RemoveAddresses: {RemoveAddresses}");
            Console.WriteLine($"OutputFilePath: {OutputFilePath}");

            var source = LoadCsv<AddressCsv>.LoadFrom(SourceAddresses);
            var remove = LoadCsv<AddressCsv>.LoadFrom(RemoveAddresses);

            List<int> removeIds = remove
               .Where(a => a.Address_ID != null)
               .Select(a => (int)a.Address_ID)
               .ToList();

            var results = new List<AlbaAddressImport>();
            int removed = 0;
            foreach (var address in source)
            {
                if(!removeIds.Contains(address.Address_ID ?? 0))
                {
                    results.Add(AlbaAddressImport.From(address));
                }
                else
                {
                    removed++;
                }
            }

            Console.WriteLine($"Removed: {removed}");

            LoadCsv<AlbaAddressImport>.SaveTo(results, OutputFilePath);

            return 0;
        }
    }
}
