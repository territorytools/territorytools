using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class AssignmentHistoryReport
    {
        public List<AssignmentRecord> Records = new List<AssignmentRecord>();
    }

    public class AssignmentRecord
    {
        public string TerritoryNumber { get; set; }
        public DateTime? Date { get; set; }
        public string PublisherName { get; set; }
        public string CheckedOut { get; set; }
        public string CheckedIn { get; set; }
        public string Note { get; set; }
    }
}