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
       "add-phone-numbers",
       HelpText = "Add phone numbers to Alba CSV")]
    public class AddPhoneNumbersOptions : IOptionsWithRun
    {
        [Option(
           "phone-numbers",
           Required = true,
           HelpText = "Input phone number file path from list services (List Services/Alba +Phones1, Phone2 CSV)")]
        [Value(0)]
        public string PhoneNumbers { get; set; }
        
        [Option(
          "addresses",
          Required = true,
          HelpText = "Input latest Alba address file path (Alba CSV)")]
        [Value(0)]
        public string Addresses { get; set; }

        [Option(
            "output",
            Required = true,
            HelpText = "Output Alba address file path (Alba CSV)")]
        [Value(0)]
        public string OutputFilePath { get; set; }

        [Option(
           "phone-territory-id",
           Required = false,
           HelpText = "Id of territory to move addresses to that have phone numbers.")]
        [Value(0)]
        public int PhoneTerritoryId { get; set; }

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
            Console.WriteLine("Loading List Services address with phone numbers records...");
            Console.WriteLine($"PhoneNumbers: {PhoneNumbers}");
            Console.WriteLine($"Addresses: {Addresses}");

            var phoneNumbers = LoadCsv<AddressCsv>.LoadFrom(PhoneNumbers);
            var addresses = LoadCsv<AlbaAddressExport>.LoadFrom(Addresses);

            Console.WriteLine($"Phone Number Record Count: {phoneNumbers.Count()}");

            var results = AddressCsvLoader.AddPhoneNumbers1And2(
                numbers: phoneNumbers,
                addresses: addresses,
                territoryId: PhoneTerritoryId);

            foreach (var address in results.SuccessfulAddresses)
            {
                Console.WriteLine($"{address.Address}: {address.Notes}");
            }

            Console.WriteLine("Errors:");
            foreach(var error in results.Errors)
            {
                Console.WriteLine($"{error.Address_ID}: {error.Address} {error.Phone1}, {error.Phone2}");
            }

            LoadCsvAddresses.SaveTo(results.SuccessfulAddresses, OutputFilePath);

            return 0;
        }
    }
}
