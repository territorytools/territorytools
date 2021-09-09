using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace TerritoryTools.Identity.Service.Entities
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string  Mobile { get; set; }
        public string  Group { get; set; }
        public string  SubGroup { get; set; }
        public bool IsActive { get; set; }
    }
}