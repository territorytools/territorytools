using System.Collections.Generic;

namespace WebUI
{
    public class SummarizeCompletedReport
    {
        public List<SummaryItem> SummaryItems = new List<SummaryItem>();
    }

    public class SummaryItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}