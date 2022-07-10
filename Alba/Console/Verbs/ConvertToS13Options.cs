using CommandLine;
using CommandLine.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb("convert-to-s-13", HelpText = "Convert Backup Folders to S-13")]
    public class ConvertToS13Options : IOptionsWithRun
    {
        [Option("folder-path", Required = true, HelpText = "Folder paths")]
        public string FolderPath { get; set; }
        
        [Option("output-file", HelpText = "Output file path")]
        public string OutputFile { get; set; }

        [Option("one-row", HelpText = "One row per territory, last row only")]
        public bool OneRow { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Convert Backup Folders to S-13 example", 
                        new ConvertToS13Options {
                            FolderPath = "C:\\BackUp\\Folder" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Converting folder backups to S-13 form...");

            Console.WriteLine($"Folder Path: {FolderPath}");

            Console.WriteLine("Loading folders...");
            Console.WriteLine("Removing entries with Checked-In and Checked-Out both blank...");
            try
            {
                List<S13Entry> entries = BackupFolder.LoadFolder(FolderPath);

                if(OneRow)
                {
                    entries = entries
                        .GroupBy(e => e.Number)
                        .Select(e => e.Last())
                        .ToList();
                }

                Console.WriteLine($"Entries Loaded: {entries.Count}");
                if (string.IsNullOrWhiteSpace(OutputFile))
                {
                    foreach (var entry in entries)
                    {
                        var row = new S13EntryCsvRow(entry);
                        Console.WriteLine($"{row}");
                    }
                }
                else
                {
                    var options = new TypeConverterOptions 
                    {
                        Formats = new[] { "yyyy-MM-dd" } 
                    };

                    var configuration = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                    {
                        Delimiter = ","
                    };

                    using (var writer = File.CreateText(OutputFile))
                    {
                        var csvWriter = new CsvWriter(writer, configuration);
                        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

                        csvWriter.WriteRecords(entries);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}
