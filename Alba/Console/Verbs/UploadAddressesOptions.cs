using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
       "upload-addresses",
       HelpText = "Upload addresses one-by-one to Alba with a delay")]
    public class UploadAddressesOptions
    {
        [Option(
           "addresses-file",
           Required = true,
           HelpText = "Input file path from source Alba address export (tab separated)")]
        [Value(0)]
        public string AddressesFile { get; set; }

        [Option(
          "languages-file",
          Required = true,
          HelpText = "Input file path from removed Alba address CSV")]
        [Value(0)]
        public string LanguagesFile { get; set; }

        [Option(
        "upload-delay-ms",
        Required = false,
        HelpText = "How long to pause between each upload")]
        [Value(0)]
        public int UploadDelayMs { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Upload addresses file example",
                        new UploadAddressesOptions {
                            AddressesFile = "addresses.csv",
                            LanguagesFile = "languages.html",
                            UploadDelayMs = 100
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Uploading addresses one by one...");
            Console.WriteLine($"AddressesFile: {AddressesFile}");
            Console.WriteLine($"LanguagesFile: {LanguagesFile}");
            Console.WriteLine($"UploadDelayMs: {UploadDelayMs}");


            Console.WriteLine("Uploading addresses one-by-one...");

            var client = Program.AlbaClient();

            client.Authenticate(Program.GetCredentials());

            new ImportAddress(client, msDelay: 100)
                   .Upload(AddressesFile, LanguagesFile);

            Console.WriteLine("Uploading addresses completed");

            return 0;
        }
    }
}
