using System;
using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Web.MainSite.Areas.UrlShortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }

        public int HostId { get; set; }

        public ShortUrlHost Host { get; set; }

        [Required]
        public string Path { get; set; }
        
        [Required]
        public string OriginalUrl { get; set; }

        public string UserName { get; set; }
        
        public string Subject { get; set; }
        
        public string LetterLink { get; set; }
        
        public string Note { get; set; }

        public DateTime Created { get; set; } = DateTime.MinValue;
    }
}
