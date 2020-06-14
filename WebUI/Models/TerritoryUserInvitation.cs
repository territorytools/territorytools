using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class TerritoryUserInvitation
    {
        public string Email { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
    }
}
