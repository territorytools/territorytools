using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "download-assignments", 
        HelpText = "Download territory assignments using your account")]
    public class DownloadAssignmentsOptions
    {
        [Option(
            "filepath", 
            Required = true, 
            HelpText = "Output file path (Alba CSV)")]
        [Value(0)]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Download example",
                        new DownloadAssignmentsOptions {
                            OutputFilePath = "file.csv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Downloading assignments...");

            var client = Program.AlbaClient();

            client.Authenticate(Program.GetCredentials());

            var useCase = new DownloadTerritoryAssignments(client);

            useCase.SaveAs(OutputFilePath);

            return 0;
        }

    }
}
