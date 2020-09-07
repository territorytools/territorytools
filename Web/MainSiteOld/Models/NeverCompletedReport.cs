using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerritoryTools.Alba.Controllers.UseCases;
using Controllers.UseCases;

namespace TerritoryTools.Web.MainSite
{
    public class NeverCompletedReport
    {
        public List<User> Publishers { get; set; } 
            = new List<User>();

        public List<Assignment> Assignments { get; set; } 
            = new List<Assignment>();
    }
}