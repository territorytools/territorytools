using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;
using Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Models
{
    public class AssignSingleTerritoryForm
    {
        public List<User> Users { get; set; } 
            = new List<User>();

        public AlbaAssignmentValues Assignment { get; set; } 
    }
}