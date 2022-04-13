using Controllers.UseCases;
using System.Collections.Generic;
using TerritoryTools.Entities;

namespace TerritoryTools.Web.MainSite.Models
{
    public class ManagePhoneTerritoryIndexPage
    {
        public string DefaultSourceDocumentId { get; set; }
        public string DefaultSourceSheetName { get; set; }
        public List<TerritoryUser> Users { get; set; }
            = new List<TerritoryUser>();
        public List<Area> Areas { get; set; } = new List<Area>();
    }
}