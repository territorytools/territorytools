using System;
using CsvHelper.Configuration.Attributes;

namespace Controllers.S13
{
    public class AssignmentRow
    {
        public string Territory { get; set; }
        public int Entry { get; set; }
        public string Publisher { get; set; }

        [Name("Checked In")]
        public DateTime? CheckedIn { get; set; }

        [Name("Checked Out")]
        public DateTime? CheckedOut { get; set; }
    }
}
