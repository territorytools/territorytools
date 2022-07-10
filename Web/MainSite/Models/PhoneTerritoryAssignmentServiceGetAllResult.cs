using System.Collections.Generic;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Models
{
    public class PhoneTerritoryAssignmentServiceGetAllResult
    {
        public bool Success { get; internal set; }
        public List<PhoneTerritoryAssignment> Rows { get; internal set; }
    }
}
