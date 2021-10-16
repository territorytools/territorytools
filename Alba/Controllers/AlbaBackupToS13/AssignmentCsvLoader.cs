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
                    TerritoryNumber = value.Number,
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
                var current = changes[i];
                var entry = new S13Entry
                {
                    Publisher = current.Publisher,
                    CheckOut = (current.Status == AssignmentStatus.CheckedOut
                        ? current.Date
                        : (DateTime?)null),
                    CheckIn = (current.Status == AssignmentStatus.CheckedIn
                        ? current.Date
                        : (DateTime?)null),
                };

                if (i == (changes.Count - 1))
                {
                    entries.Add(entry);
                    continue;
                }
                
                var next = changes[i + 1];
                if(current.TerritoryNumber.TrimStart(' ', '0').ToUpper()
                    != next.TerritoryNumber.TrimStart(' ', '0').ToUpper())
                {
                    entries.Add(entry);
                    continue;
                }

                if(current.Status == AssignmentStatus.CheckedOut
                    && next.Status == AssignmentStatus.CheckedIn)
                {
                    entry.Publisher = next.Publisher;
                    entry.CheckIn = next.Date;
                    entries.Add(entry);
                    i++; // Skip next entry because we merged it with current
                    continue;
                }

                if (current.Status == AssignmentStatus.CheckedIn)
                {
                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
