﻿using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "download-addresses", 
        HelpText = "Download addresses using your account")]
    public class DownloadAddressesOptions : IOptionsWithRun
    {
        [Option("filepath", Required = true, HelpText = "Input file path")]
        [Value(0)]
        public string FilePath { get; set; }

        [Option("accountid", Required = true, HelpText = "Input account id (In Alba click Account, ID is on the far right, example: 123)")]
        [Value(1)]
        public int AccountId { get; set; }
        
        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Download example", 
                        new DownloadAddressesOptions { 
                            FilePath = "file.csv" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Downloading addresses...");

            var client = Program.AlbaClient();

            client.Authenticate(Program.GetCredentials());

            var exporter = new DownloadAddressExport(client);

            exporter.SaveAs(FilePath, AccountId);

            return 0;
        }
    }
}
