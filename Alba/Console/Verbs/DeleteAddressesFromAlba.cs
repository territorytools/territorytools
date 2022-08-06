using CommandLine;
using CommandLine.Text;
using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "delete-addresses-from-alba", 
        HelpText = "Delete addresses using your account")]
    public class DeleteAddressesFromAlbaOptions : IOptionsWithRun
    {
        [Option(
           "address-file",
           Required = true,
           HelpText = "Input file with lists of addresses to delete, path from source Alba address export (tab separated)")]
        [Value(0)]
        public string AddressFile { get; set; }

        [Option("accountid", Required = true, HelpText = "Input account id (In Alba click Account, ID is on the far right, example: 123)")]
        [Value(1)]
        public int AccountId { get; set; }

        [Option(
           "upload-delay-ms",
           Required = false,
           HelpText = "How long to pause between each upload")]
        [Value(2)]
        public int DelayMs { get; set; } = 500;

        [Option(
           "pipe",
           Required = false,
           HelpText = "Read from pipe")]
        [Value(3)]
        public bool Pipe { get; set; }

        [Option(
          "dry-run",
          Required = false,
          HelpText = "Don't really run")]
        public bool DryRun { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Download example", 
                        new DeleteAddressesFromAlbaOptions {
                            AddressFile = "addresses.csv" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Deleting addresses...");

            if (Pipe)
            {
                while (true)
                {
                    string line = Console.In.ReadLine();
                    if (line == null)
                        break;
                    Console.WriteLine($"Line: {line}");
                }
                return 0;
            }

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                BadDataFound = null,
                //PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            var client = Program.AlbaClient();
            client.Authenticate(Program.GetCredentials());

            using (var reader = new StreamReader(AddressFile))
            using (var csv = new CsvReader(reader, configuration))
            {
                var addresses = csv.GetRecords<AlbaAddressExport>().ToList();
                int count = addresses.Count;
                int row = 0;

                foreach (AlbaAddressExport address in addresses)
                {
                    Console.WriteLine(address);
                    if (address.Address_ID != null && address.Address_ID != 0)
                    {
                        Console.WriteLine($"Deleting address id \"{address.Address_ID}\" in territory {address.Territory_number}");
                        if (!DryRun)
                        {
                            client.DownloadString(RelativeUrlBuilder.DeleteAddress(address.Address_ID ?? 0));
                        }
                        Console.WriteLine($"Row {row++}/{count-1} Pausing {DelayMs}ms...");
                        Thread.Sleep(DelayMs);
                    }
                }
            }
            return 0;
        }
    }
}
