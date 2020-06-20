using System.Collections.Generic;
using Alba.Controllers.UseCases;
using Controllers.UseCases;

namespace WebUI
{
    public class AssignSingleTerritoryForm
    {
        public List<User> Users { get; set; } 
            = new List<User>();

        public Assignment Assignment { get; set; } 
    }
}