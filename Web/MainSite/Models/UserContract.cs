using System;

namespace TerritoryTools.Web.MainSite.Models
{
    public class UserContract
    {
        public int Id { get; set; }
        public string? AlbaFullName { get; set; }
        public string? AlbaEmail { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? Phone { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public string? Notes { get; set; }
        public string? GroupId { get; set; }
        public string? SubGroupId { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool CanAssignTerritories { get; set; }
        public string? AlbaUserId { get; set; }
    }
}
