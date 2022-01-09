using System.Collections.Generic;
using TerritoryTools.Web.MainSite.Controllers;

namespace TerritoryTools.Web.MainSite.Models
{
    public class AddressSearchPage
    {
        public List<AddressSearchResult> Addresses { get; set; }
            = new List<AddressSearchResult>();
    }
}