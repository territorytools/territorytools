using System;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class AssignmentValues
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public string Status { get; set; }
        public DateTime? LastCompleted { get; set; }
        public string LastCompletedBy { get; set; }
        public DateTime? SignedOut { get; set; }
        public string SignedOutString { get; set; }
        public string SignedOutTo { get; set; }
    }
}
