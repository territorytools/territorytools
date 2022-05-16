using CommandLine;
using System;
using System.IO;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "assign-sheet", 
        HelpText = "Assign an exsiting territory sheet to a user")]
    public class AssignSheetOptions : IOptionsWithRun
    {
        [Option("document-id", Required = true, HelpText = "Input Google Sheet document ID")]
        [Value(1)]
        public string DocumentId { get; set; }

        [Option("publisher-email", Required = true, HelpText = "Email address of publisher")]
        [Value(4)]
        public string PublisherEmail { get; set; }

        [Option("owner-email", Required = true, HelpText = "Email address of new document owner")]
        [Value(5)]
        public string OwnerEmail { get; set; }

        [Option("security-file", Required = true, HelpText = "Path to security JSON file from Google.")]
        [Value(6)]
        public string SecurityFile { get; set; }

        public int Run()
        {
            Console.WriteLine($"Extracting territory from document {DocumentId}...");

            string jsonToken = File.ReadAllText(SecurityFile);

            var service = new SheetExtractor();
            var request = new AssignSheetRequest()
            {
                DocumentId = DocumentId,
                PublisherEmail = PublisherEmail,
                OwnerEmail = OwnerEmail,
                SecurityToken = jsonToken
            };

            string url = service.Assign(request);

            System.Diagnostics.Process.Start("explorer.exe", url);

            return 0;
        }
    }
}
