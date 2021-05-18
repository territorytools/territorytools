using System;
using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Entities
{
    public class TerritoryAssignment
    {
        [Key]
        public Guid Id { get; set; }
        public string TerritoryNumber { get; set; }
        public string PublisherName { get; set; }
        public DateTime? CheckedOut { get; set; }
        public DateTime? CheckedIn { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatedByUser { get; set; }
        public DateTime Updated { get; set; }
        public Guid UpdatedByUser { get; set; }
    }
}
