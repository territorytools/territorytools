using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;
using Controllers.UseCases;

namespace TerritoryTools.Web.MainSite
{
    public class AssignSingleTerritoryForm
    {
        public List<User> Users { get; set; } 
            = new List<User>();

        public Assignment Assignment { get; set; } 
    }
}