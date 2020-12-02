using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite
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