using CommandLine;
using System;
using System.IO;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "extract-sheet", 
        HelpText = "Extract territory sheet from master Google Sheet")]
    public class ExtractSheetOptions
    {
        [Option("territory-number", Required = true, HelpText = "Territory number to exctract")]
        [Value(0)]
        public string TerritoryNumber { get; set; }

        [Option("document-id", Required = true, HelpText = "Input Google Sheet document ID")]
        [Value(1)]
        public string DocumentId { get; set; }

        [Option("sheet-name", Required = true, HelpText = "Input Google Sheets sheet name")]
        [Value(2)]
        public string SheetName { get; set; }


        [Option("publisher", Required = true, HelpText = "Name of publisher")]
        [Value(3)]
        public string PublisherName { get; set; }

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
            var request = new SheetExtractionRequest()
            {
                FromDocumentId = DocumentId,
                FromSheetName = SheetName,
                TerritoryNumber = TerritoryNumber,
                PublisherName = PublisherName,
                PublisherEmail = PublisherEmail,
                OwnerEmail = OwnerEmail,
                SecurityToken = jsonToken
            };

            string url = service.Extract(request);

            System.Diagnostics.Process.Start("explorer.exe", url);

            return 0;
        }
    }
}
