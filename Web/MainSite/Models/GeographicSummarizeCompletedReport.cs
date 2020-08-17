using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite
{
    public class GeographicSummaryReport
    {
        public List<GeographicSummaryPeriod> Periods 
            = new List<GeographicSummaryPeriod>();
    }

    public class GeographicSummaryPeriod
    {
        public string Period { get; set; }
        public int Completed { get; set; }

        public List<GeographicSummaryZone> Zones { get; set; }
           = new List<GeographicSummaryZone>();
        public int Addresses { get; internal set; }
    }

    public class GeographicSummaryZone
    {
        public string Zone { get; set; }
        public int Completed { get; set; }

        public List<GeographicSummaryArea> Areas { get; set; }
           = new List<GeographicSummaryArea>();
        public int Addresses { get; internal set; }
    }

    public class GeographicSummaryArea
    {
        public string Area { get; set; }
        public int Completed { get; set; }
        public int Addresses { get; internal set; }
    }
}