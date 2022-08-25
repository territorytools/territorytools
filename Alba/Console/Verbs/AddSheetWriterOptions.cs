using CommandLine;
using System;
using System.IO;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "add-writer", 
        HelpText = "Add user as writer to territory sheet (Google Sheet)")]
    public class AddSheetWriterOptions : IOptionsWithRun
    {
        [Option("document-id", Required = true, HelpText = "Input Google Sheet document ID")]
        [Value(1)]
        public string DocumentId { get; set; }

        [Option("user-email", Required = true, HelpText = "Email address of user to assign as writer")]
        [Value(4)]
        public string UserEmail { get; set; }

        [Option("security-file", Required = true, HelpText = "Path to security JSON file from Google.")]
        [Value(6)]
        public string SecurityFile { get; set; }

        public int Run()
        {
            Console.WriteLine($"Adding user {UserEmail} document {DocumentId}...");

            string jsonToken = File.ReadAllText(SecurityFile);

            var sheets = new GoogleSheets("JSON is missing here");
            Console.WriteLine("Missing JSON !");
            var service = new SheetExtractor(sheets);
            var request = new AddSheetWriterRequest()
            {
                DocumentId = DocumentId,
                UserEmail = UserEmail,
                SecurityToken = jsonToken
            };

            string url = service.AddSheetWriter(request);

            System.Diagnostics.Process.Start("explorer.exe", url);

            return 1; // TODO: Fix this, see JSON above
        }
    }
}
