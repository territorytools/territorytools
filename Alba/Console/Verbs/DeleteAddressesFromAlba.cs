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
        [Value(0)]
        public int UploadDelayMs { get; set; } = -1;

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
            
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //Delimiter = "\t",
                BadDataFound = null,
                //PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            var client = Program.AlbaClient();
            client.Authenticate(Program.GetCredentials());

            using (var reader = new StreamReader(AddressFile))
            using (var csv = new CsvReader(reader, configuration))
            {
                var addresses = csv.GetRecords<AlbaAddressImport>().ToList();
                foreach(AlbaAddressImport address in addresses)
                {
                    Console.WriteLine(address);
                    if (address.Address_ID != null && address.Address_ID != 0)
                    {
                        Console.WriteLine($"Deleting address id {address.Address_ID}");
                        //client.DownloadString(RelativeUrlBuilder.DeleteAddress(address.Address_ID ?? 0));
                    }
                }
            }
            return 0;
        }
    }
}
