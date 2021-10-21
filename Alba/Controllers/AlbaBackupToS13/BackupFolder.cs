using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var allChanges = new List<AssignmentChange>();
            for (int i = 0; i < paths.Length; i++)
            {
                string file = paths[i];
                List<AssignmentValues> values2 = AssignmentValues
                  .LoadFromCsv(file);

                List<AssignmentChange> changes2 = AssignmentChange
                    .Load(values2, file);

                allChanges.AddRange(changes2);
            }

            var orderedChanges = allChanges
                .OrderBy(c => c.TerritoryNumber.TrimStart(' ', '0').ToUpper())
                .ThenBy(c => c.TimeStamp)
                .ThenBy(c => c.Date)
                .ThenBy(c => c.Status)
                .ToList();

            List<S13Entry> entries = S13EntryConverter
                   .Convert(orderedChanges);

            return entries;
        }
    }
}
