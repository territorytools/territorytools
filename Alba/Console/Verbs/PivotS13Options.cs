using CommandLine;
using CommandLine.Text;
using Controllers.S13;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb("pivot-s-13", HelpText = "Pivot S-13 form")]
    public class PivotS13Options
    {
        [Option("input-path", Required = true, HelpText = "Input file path")]
        public string InputPath { get; set; }

        [Option("alba-territory-assignments-path", Required = true, HelpText = "Path to Alba territory assignments file")]
        public string AlbaTerritoryAssignmentsPath { get; set; }

        [Option("output-path", Required = true, HelpText = "Output file path")]
        public string OutputPath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Pivot S-13 example", 
                        new PivotS13Options {
                            InputPath = "file.csv" ,
                            AlbaTerritoryAssignmentsPath = "assignments.csv",
                            OutputPath = "output.csv"
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Pivoting S-13 form...");

            Console.WriteLine($"Input File Path: {InputPath}");
            Console.WriteLine($"Alba Assignment File Path: {AlbaTerritoryAssignmentsPath}");
            Console.WriteLine($"Output File Path: {OutputPath}");

            Console.WriteLine("Loading S-13 'By Entry' Entries...");
            var rows = PivotAssignmentRowsToS13Columns.LoadFrom(InputPath);

            Console.WriteLine("Removing entries with Checked-In and Checked-Out both blank...");
            var cleanedRaw = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.CheckedIn)
                    || !string.IsNullOrWhiteSpace(r.CheckedOut))
                .ToList();

            var cleaned = new List<AssignmentRow>();
            foreach(var raw in cleanedRaw)
            {
                DateTime? checkedOutN = null;

                if(DateTime.TryParse(raw.CheckedOut, out DateTime checkedOut))
                {
                    checkedOutN = checkedOut;
                }

                DateTime.TryParse(raw.CheckedIn, out DateTime checkedIn);
            }

            Console.WriteLine("Loading assignments from Alba...");
            var assignments = DownloadTerritoryAssignments.LoadFromCsv(AlbaTerritoryAssignmentsPath);

            // TODO: Add new checked-in checked-out here
            foreach (var assignment in assignments)
            {
                // TODO: Fixing issue with one null territory having 1300+ entries
                DateTime.TryParse(assignment.SignedOutString, out DateTime signedOut);
                DateTime.TryParse(assignment.LastCompleted, out DateTime signedIn);

                if (!cleaned.Exists(c => 
                    string.Equals(assignment.Number, c.Territory, StringComparison.OrdinalIgnoreCase)
                    && assignment.SignedOutString == c.CheckedOut
                    && assignment.LastCompleted == c.CheckedIn
                    ))
                {
                    cleaned.Add(
                        new AssignmentRowRaw
                        {
                            Territory = "",
                            Publisher = "",
                            Entry = 0,
                            CheckedIn = null,
                            CheckedOut = null
                        });
                }
            }

            Console.WriteLine("Pivoting...");
            var columns = PivotAssignmentRowsToS13Columns.PivotFrom(cleaned);

            Console.WriteLine("Adding unworked territories...");
            var errors = new List<string>();
            foreach (var assignment in assignments)
            {
                try
                {
                    int assignmentNumber = int.Parse(assignment.Number);

                    if (!columns.Exists(c => string.Equals(c.Territory, assignmentNumber.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var newCol = new S13Column
                        {
                            Territory = assignment.Number
                        };

                        newCol.Entries.Add(new S13Entry { Publisher = "Never Worked" });
                        columns.Add(newCol);
                    }
                }
                catch (Exception e)
                {
                    errors.Add($"Number: {assignment.Number}: {e.Message}");
                }
            }

            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }

            var orderedColumns = columns.OrderBy(c => int.Parse(c.Territory)).ToList();

            Console.WriteLine("Saving data to new file...");
            
            PivotAssignmentRowsToS13Columns.SaveTo(orderedColumns, OutputPath);

            return 0;
        }
    }
}
