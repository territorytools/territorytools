using Controllers.UseCases;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class SingleTerritoryManagerPageV2
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string MobileLink { get; set; }
        public string SignedOutTo { get; set; }
        public string SignedOut { get; set; }
        public string LastCompletedBy { get; set; }
        public string LastCompleted { get; set; }
        public string Kind { get; set; }
        public int Addresses { get; set; }
        public string Status { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public string PrintLink { get; internal set; }
    }
}
