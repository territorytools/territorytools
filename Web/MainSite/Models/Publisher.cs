using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Models
{
    public class Publisher
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<AlbaAssignmentValues> Territories { get; set; } 
            = new List<AlbaAssignmentValues>();
        public List<QRCodeHit> QRCodeActivity { get; set; }
            = new List<QRCodeHit>();
    }
}
