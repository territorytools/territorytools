using System;
using CsvHelper.Configuration.Attributes;

namespace Controllers.S13
{
    public class TerritoryLastCompleted
    {
        public string Territory { get; set; }
        public int TimesWorked { get; set; }
        public string Publisher { get; set; }

        [Name("Checked Out")]
        public DateTime? CheckedOut { get; set; }

        [Name("Checked In")]
        public DateTime? CheckedIn { get; set; }
    }
}
