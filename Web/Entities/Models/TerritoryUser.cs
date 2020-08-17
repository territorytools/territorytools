using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Web.MainSite.Models
{
    public class TerritoryUser 
    {
        [Key]
        public virtual Guid Id { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public virtual string AspNetUserId { get; set; }
        public List<TerritoryUserAlbaAccountLink> AlbaAccountLinks { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? LastSignIn { get; set; }
        public virtual DateTime Updated { get; set; }
    }
}
