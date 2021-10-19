using System;
using System.Collections.Generic;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class S13EntryConverter
    {
        public static List<S13Entry> Convert(
            IList<AssignmentChange> changes)
        {
            var entries = new List<S13Entry>();
            for (int i = 0; i < changes.Count; i++)
            {
                var current = changes[i];
                var entry = new S13Entry
                {
                    Number = current.TerritoryNumber,
                    Publisher = current.Publisher,
                    CheckedOut = (current.Status == AssignmentStatus.CheckedOut
                        ? current.Date
                        : (DateTime?)null),
                    CheckedIn = (current.Status == AssignmentStatus.CheckedIn
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
                    entry.CheckedIn = next.Date;
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
