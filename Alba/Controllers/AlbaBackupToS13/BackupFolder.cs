using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class BackupFolder
    {
        public static List<S13Entry> LoadFolder(string path)
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

        static List<S13Entry> ConvertFiles(string[] paths)
        {
            var allChanges = new List<AssignmentChange>();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                List<AssignmentValues> values = AssignmentValues
                    .LoadFromCsv(path);

                List<AssignmentChange> changes = AssignmentChange
                    .Load(values, path);

                allChanges.AddRange(changes);
            }

            List<AssignmentChange> orderedChanges = allChanges
                .OrderBy(c => c.TerritoryNumber, new NumberComparer())
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
