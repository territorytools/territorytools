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

    }
}
