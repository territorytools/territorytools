using System;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AlbaAssignment
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public string Status { get; set; }
        public DateTime? LastCompleted { get; set; }
        public string LastCompletedBy { get; set; }
        public DateTime? SignedOut { get; set; }
        public string SignedOutTo { get; set; }
        public string MobileLink { get; set; }
    }
}
