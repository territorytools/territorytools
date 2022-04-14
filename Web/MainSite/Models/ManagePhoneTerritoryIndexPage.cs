using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class ManagePhoneTerritoryIndexPage
    {
        public string DefaultSourceDocumentId { get; set; }
        public string DefaultSourceSheetName { get; set; }
        public List<ManagePhoneTerritorIndexPageUser> Users { get; set; }
            = new List<ManagePhoneTerritorIndexPageUser>();
    }

    public class ManagePhoneTerritorIndexPageUser
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
    }
}