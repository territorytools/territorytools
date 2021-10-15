using Controllers.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class AssignmentCsvLoader
    {
        public static List<AssignmentValues> LoadFromCsv(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var csv = LoadCsv.LoadFrom<AssignmentValues>(path);
            
            return csv.ToList();
        }

        public static List<AssignmentChange> Load(string path)
        {
            var values = LoadFromCsv(path);
            var changes = new List<AssignmentChange>();
            foreach(var value in values)
            {
                var change = new AssignmentChange
                {
                    Date = (DateTime)(value.LastCompleted ?? value.SignedOut),
                    Publisher = value.LastCompleted == null
                        ? value.SignedOutTo
                        : value.LastCompletedBy,
                    Status = value.LastCompleted == null
                        ? AssignmentStatus.CheckedOut
                        : AssignmentStatus.CheckedIn
                };

                changes.Add(change);
            }

            return changes;
        }

        public static List<S13Entry> LoadS13Entries(string path)
        {
            List<AssignmentChange> changes = Load(path);

            var entries = new List<S13Entry>();
            for (int i = 0; i < changes.Count; i++)
            {
                if (i == 0)
                {
                    var entry = new S13Entry
                    {
                        Publisher = changes[i].Publisher,
                        CheckOut = (changes[i].Status == AssignmentStatus.CheckedOut ? changes[i].Date : (DateTime?)null),
                        CheckIn = (changes[i].Status == AssignmentStatus.CheckedIn ? changes[i].Date : (DateTime?)null),
                    };

                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
