using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Models
{
    public class TerritoryContract
    {
        public int Id { get; set; }
        public int? AlbaTerritoryId { get; set; }
        public string? Number { get; set; }
        public string? Description { get; set; }
        public string? AreaDescription { get; set; }
        public string? AreaCode { get; set; }
        public string? AreaCodeSuffix { get; set; }
        public int? AlbaKind { get; set; }
        public int AddressCount { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastCompleted { get; set; }
        public string? LastCompletedBy { get; set; }
        public DateTime? SignedOut { get; set; }
        public string? SignedOutString { get; set; }
        public string? SignedOutTo { get; set; }
        public int? SignedOutToAlbaUserId { get; set; }
        public int? MonthsSignedOut { get; set; }
        public int? MonthsAgoCompleted { get; set; }
        public string? AlbaMobileLink { get; set; }
        public string? AlbaPrintLink { get; set; }
        public string? AlbaMobileTerritoryKey { get; set; }
        public string? AssigneeLinkKey { get; set; }
        public string? AssigneeMobileLink { get; set; }
        public string? ActiveLinkKey { get; set; }
        public List<double[]> Border { get; set; } = new();
    }
}
