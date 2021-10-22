using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class BackupFolder
    {
        public static S13EntryCollection LoadFolder(string path)
        {
            return ConvertFiles(LoadFolders(path));
        }

        static string[] LoadFolders(string path)
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

        static S13EntryCollection ConvertFiles(string[] paths)
        {
            var allChanges = new List<AssignmentChange>();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                List<AssignmentValues> values = AssignmentValues
                    .LoadFromCsv(path);

                List<AssignmentChange> changes = AssignmentChange
                    .Load(values, path);

                var duplicates = changes
                    // Exclude seeding folder
                    .Where(c => c.TimeStamp != DateTime.Parse("1900-01-01"))
                    .GroupBy(c => c.TerritoryNumber)
                    .Where(g => g.Count() > 1)
                    .ToDictionary(c => c.Key);

                var uniques = new List<AssignmentChange>();
                foreach(var change in changes)
                {
                    if (duplicates.ContainsKey(change.TerritoryNumber))
                    {
                        uniques.Add(duplicates[change.TerritoryNumber].First());
                    }
                    else
                    {
                        uniques.Add(change);
                    }
                }

                allChanges.AddRange(uniques);
            }

            List<AssignmentChange> orderedChanges = allChanges
                .OrderBy(c => c.TerritoryNumber, new NumberComparer())
                .ThenBy(c => c.TimeStamp)
                .ThenBy(c => c.Date)
                .ThenBy(c => c.Status)
                .ToList();

            S13EntryCollection entries = S13EntryConverter
                   .Convert(orderedChanges);

            return entries;
        }
    }
}
