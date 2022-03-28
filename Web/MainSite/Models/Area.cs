namespace TerritoryTools.Web.MainSite.Models
{
    public class Area
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }

        public bool IsParent { get; set; }
    }
}
