using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbaClient.Controllers.UseCases;
using Controllers.UseCases;

namespace WebUI
{
    public class NeverCompletedReport
    {
        public List<User> Publishers { get; set; } 
            = new List<User>();

        public List<Assignment> Assignments { get; set; } 
            = new List<Assignment>();
    }
}