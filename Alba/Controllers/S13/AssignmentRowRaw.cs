using CsvHelper.Configuration.Attributes;

namespace Controllers.S13
{
    public class AssignmentRowRaw
    {
        public string Territory { get; set; }
        public int Entry { get; set; }
        public string Publisher { get; set; }

        [Name("Checked In")]
        public string CheckedIn { get; set; }

        [Name("Checked Out")]
        public string CheckedOut { get; set; }
    }
}
