using System;

namespace TerritoryTools.Alba.Controllers.AlbaBackupToS13
{
    public class S13EntryCsvRow
    {
        public S13EntryCsvRow(S13Entry entry)
        {
            Number = entry.Number;
            Publisher = entry.Publisher;
            CheckedOut = entry.CheckedOut == null 
                ? null 
                : ((DateTime)entry.CheckedOut).ToString("yyyy-MM-dd");
            CheckedIn = entry.CheckedIn == null 
                ? null 
                : ((DateTime)entry.CheckedIn).ToString("yyyy-MM-dd");
        }

        public string Number { get; set; }
        public string Publisher { get; set; }
        public string CheckedOut { get; set; }
        public string CheckedIn { get; set; }
        public override string ToString()
        {
            return $"{Number} {Publisher} Out:{CheckedOut} In:{CheckedIn}";
        }
    }
}
