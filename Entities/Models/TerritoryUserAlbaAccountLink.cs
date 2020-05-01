using System;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class TerritoryUserAlbaAccountLink
    {
        [Key]
        public int TerritoryUserAlbaAccountLinkId { get; set; }
        public int TerritoryUserId { get; set; }
        public int AlbaAccountId { get; set; }
        public string Role { get; set; }
        public TerritoryUser TerritoryUser { get; set; }
        public AlbaAccount AlbaAccount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime Updated { get; set; }
    }
}
