using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class PhoneTerritoryCreateResult
    {
        public bool Success { get; internal set; }
        public string Message { get; set; }
        public List<PhoneTerritoryCreateItem> Items { get; set; } = new List<PhoneTerritoryCreateItem>();
    }
}
