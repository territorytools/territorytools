using Controllers.UseCases;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class ReportIndexPage
    {
        public List<User> Users { get; set; }
            = new List<User>();
        public List<Area> Areas { get; set; } = new List<Area>();
    }
}