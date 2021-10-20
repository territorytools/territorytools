using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            IEnumerable<AssignmentValues> values,
            string path)
        {
            var changes = new List<AssignmentChange>();

            string folderName = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar).Last();
            if (!Regex.IsMatch(folderName, @"\d{4}-\d{2}-\d{2}_\d{6}"))
                return changes;

            string datePart = folderName.Substring(0, 10);
            string hh = folderName.Substring(11, 2);
            string mm = folderName.Substring(13, 2);
            string ss = folderName.Substring(15, 2);
            string formatted = $"{datePart}T{hh}:{mm}:{ss}";

            if(!DateTime.TryParse(formatted, out DateTime folderDate))
                return changes;

            foreach (var value in values)
            {
                var change = new AssignmentChange
                {
                    TimeStamp = folderDate,
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
