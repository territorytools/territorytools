using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Models
{
    public class Publisher
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public List<AlbaAssignmentValues> Territories { get; set; } 
            = new List<AlbaAssignmentValues>();
        public List<QRCodeHit> QRCodeActivity { get; set; }
            = new List<QRCodeHit>();
        public bool PhoneTerritorySuccess { get; set; }
    }
}
