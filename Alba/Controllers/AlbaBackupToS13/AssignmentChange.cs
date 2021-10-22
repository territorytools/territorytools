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
                if(value.LastCompleted == null && value.SignedOut == null
                    // Skip blank entries, or entries probably marked 'Gap'
                    || value.LastCompleted == DateTime.Parse("1900-01-01")
                    || value.SignedOut == DateTime.Parse("1900-01-01"))
                {
                    continue;
                }

                string numberString = value.Number;
                if(int.TryParse(value.Number, out int number))
                {
                    // TODO: This only works if the account uses 4 digit numbers with padded zeros
                    numberString = $"{number}";
                }
                try
                {
                    var change = new AssignmentChange
                    {
                        TimeStamp = folderDate,
                        TerritoryNumber = numberString,
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
                catch(Exception e)
                {
                    throw new Exception("Error converting values to changes", e);
                }

            }

            return changes;
        }

        public override string ToString()
        {
            return $"{TerritoryNumber} {TimeStamp} {Date} {Status} {Publisher}";
        }
    }
}
