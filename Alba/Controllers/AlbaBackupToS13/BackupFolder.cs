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
            int startIndex = 0;
            if(paths.Length > 0 
                && Path.GetDirectoryName(paths[0]).EndsWith($"{Path.DirectorySeparatorChar}1900-01-01_000000"))
            {
                startIndex = 1;
                List<AssignmentValues> values = AssignmentValues
                        .LoadFromCsv(paths[0]);

                List<AssignmentChange> changes = AssignmentChange
                    .Load(values, paths[0]);

                List<S13Entry> entries = S13EntryConverter
                    .Convert(changes);

                allEntries.AddRange(entries);
            }

            //for(int i = startIndex; i < paths.Length; i++)
            //{
            //    string file = paths[i];
            //    List<AssignmentValues> values2 = AssignmentValues
            //      .LoadFromCsv(file);

            //    List<AssignmentChange> changes2 = AssignmentChange
            //        .Load(values2);

            //    List<S13Entry> entries2 = S13EntryConverter
            //        .Convert(changes2);

            //    //foreach()

            //    allEntries.AddRange(entries2);
            //    break;
            //}

            return allEntries;
        }
    }
}
