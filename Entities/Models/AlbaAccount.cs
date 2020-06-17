using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class AlbaAccount
    {
        [Key]
        public Guid Id { get; set; }
        public int IdInAlba { get; set; }
        public string HostName { get; set; }
        public string AccountName { get; set; }
        public string LongName { get; set; }
        public List<AlbaUser> Users { get; set; }
        public List<TerritoryUserAlbaAccountLink> TerritoryUserLinks { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime Updated { get; set; }
    }
}
