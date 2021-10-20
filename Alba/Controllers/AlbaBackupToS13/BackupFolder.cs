using System;
using System.Collections.Generic;
using System.IO;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class BackupFolder
    {
        public static string[] Load(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception($"Folder {path} does not exist");
            }

            string[] folders = Directory.GetDirectories(path);

            var files = new List<string>();
            foreach(string folder in folders)
            {
                string filePath = Path.Combine(folder, "territories.txt");
                if(File.Exists(filePath))
                {
                    files.Add(filePath);
                }
            }

            return files.ToArray();
        }

        public static List<S13Entry> LoadStuff(string[] paths)
        {
            var allEntries = new List<S13Entry>();
            foreach (var file in paths)
            {
                List<AssignmentValues> values = AssignmentValues
                  .LoadFromCsv(file);

                List<AssignmentChange> changes = AssignmentChange
                    .Load(values);

                List<S13Entry> entries = S13EntryConverter
                    .Convert(changes);

                //foreach()

                allEntries.AddRange(entries);
                break;
            }

            return allEntries;
        }
    }
}
