using CommandLine;
using CommandLine.Text;
using Controllers.UseCases;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb("convert-to-s-13", HelpText = "Convert Backup Folders to S-13")]
    public class ConvertToS13Options : IOptionsWithRun
    {
        [Option("folder-path", Required = true, HelpText = "Folder paths")]
        public string FolderPath { get; set; }

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
                S13EntryCollection entries = BackupFolder.LoadFolder(FolderPath);
                Console.WriteLine($"Entries Loaded: {entries.Count}");
                foreach (var entry in entries)
                {
                    var row = new S13EntryCsvRow(entry);
                    Console.WriteLine($"{row}");
                    //WriteObject(new S13EntryCsvRow(entry));
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
