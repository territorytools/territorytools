using System;
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

            return folders;
            //List<S13Entry> actuals = S13EntryConverter
            //      .Convert(LoadSeedValues());

            //List<S13Entry> expecteds = S13Entry.LoadCsv("AlbaBackupToS13/expected.csv");

        }
    }

    public class DayFolder
    {

    }
}
