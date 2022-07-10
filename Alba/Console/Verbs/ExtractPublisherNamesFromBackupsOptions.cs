using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb("extract-publishers", HelpText = "Extract Publisher Names Backup Folders")]
    public class ExtractPublisherNamesFromBackupsOptions : IOptionsWithRun
    {
        [Option("folder-path", Required = true, HelpText = "Backup folder path")]
        public string FolderPath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Extract Publisher Names Backup Folders example", 
                        new ExtractPublisherNamesFromBackupsOptions {
                            FolderPath = "C:\\BackUp\\Folder" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Extract publisher names from folder backups...");

            Console.WriteLine($"Folder Path: {FolderPath}");

            Console.WriteLine("Loading folders...");
            try
            {
                S13EntryCollection entries = BackupFolder.LoadFolder(FolderPath);
                Console.WriteLine($"Entries Loaded: {entries.Count}");
                foreach (string publisher in entries.Publishers)
                {
                    Console.WriteLine(publisher);
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
