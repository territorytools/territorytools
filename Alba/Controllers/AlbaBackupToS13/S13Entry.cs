using System;
namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class S13Entry
    {
        //public DateTime TimeStamp { get; set; }
        public string Number { get; set; }
        public string Publisher { get; set; }
        public DateTime? CheckedOut { get; set; }
        public DateTime? CheckedIn { get; set; }
    }
}
