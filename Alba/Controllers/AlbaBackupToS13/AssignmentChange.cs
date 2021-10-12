using System;

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
    }
}
