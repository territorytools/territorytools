using System;
using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Entities
{
    public class TerritoryUserAlbaAccountLink
    {
        [Key]
        public int TerritoryUserAlbaAccountLinkId { get; set; }
        public Guid TerritoryUserId { get; set; }
        public Guid AlbaAccountId { get; set; }
        public string Role { get; set; }
        public TerritoryUser TerritoryUser { get; set; }
        public AlbaAccount AlbaAccount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime Updated { get; set; }
    }
}
