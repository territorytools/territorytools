using System;
namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class S13Entry
    {
        public DateTime TimeStamp { get; set; }
        public string Publisher { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
