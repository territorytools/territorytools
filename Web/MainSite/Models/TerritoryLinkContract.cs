using System;

namespace TerritoryTools.Web.MainSite.Models
{
    public class TerritoryLinkContract
    {
        public string Id { get; set; }
        public string TerritoryUri { get; set; }
        public string AlbaMobileTerritoryKey { get; set; }
        public string TerritoryNumber { get; set; }
        public string TerritoryDescription { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedById { get; set; }
        public DateTime? Expires { get; set; }
        public string AssigneeId { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneeEmail { get; set; }
        public string AssigneePhone { get; set; }
        public string GroupId { get; set; }
        public bool Successful { get; set; }
    }
}
