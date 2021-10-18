using System;
using System.Collections.Generic;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class AssignmentChange
    {
        public DateTime TimeStamp { get; set; }
        public DateTime Date { get; set; }
        public string TerritoryNumber { get; set; }
        public string Publisher { get; set; }
        public AssignmentStatus Status { get; set; }
        public string Note { get; set; }

        public static List<AssignmentChange> Load(
            IEnumerable<AssignmentValues> values)
        {
            var changes = new List<AssignmentChange>();
            foreach (var value in values)
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
    }
}
