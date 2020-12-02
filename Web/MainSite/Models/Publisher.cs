using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Models
{
    public class Publisher
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Assignment> Territories { get; set; } 
            = new List<Assignment>();
        public List<QRCodeHit> QRCodeActivity { get; set; }
            = new List<QRCodeHit>();
    }
}
