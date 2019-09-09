using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlbaClient.Controllers.UseCases;

namespace WebUI
{
    public class Publisher
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Assignment> Territories { get; set; } 
            = new List<Assignment>();
    }
}
