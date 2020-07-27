using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;
using System.Text;
using System;
using Controllers.UseCases;
using System.Linq;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
       "remove-assigned",
       HelpText = "Remove assigned territories")]
    public class RemoveAssignedTerritoriesOptions
    {
        [Option(
           "input",
           Required = true,
           HelpText = "Input file path (Alba CSV)")]
        [Value(0)]
        public string InputFilePath { get; set; }

        [Option(
            "output",
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
                        new RemoveAssignedTerritoriesOptions {
                            InputFilePath = "unassigned-only.csv",
                            OutputFilePath = "from-alba.csv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Removing Assigned Territories...");

            Console.WriteLine($"Input File Path: {InputFilePath}");
            Console.WriteLine($"Output File Path: {OutputFilePath}");

            var assignments = DownloadTerritoryAssignments.LoadFromCsv(InputFilePath);

            Console.WriteLine($"Before Count: {assignments.Count()}");

            var filtered = assignments
                .Where(a => !string.Equals(a.Status, "Signed-out", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine($"After Filter Count: {filtered.Count}");

            DownloadTerritoryAssignments.SaveAs(filtered, OutputFilePath);

            return 0;
        }
    }
}
