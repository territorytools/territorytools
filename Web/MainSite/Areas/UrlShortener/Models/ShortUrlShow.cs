using System;

namespace WebUI.Areas.UrlShortener.Models
{
    public class ShortUrlShow
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; }

        public string ShortUrl { get; set; }

        public int HitCount { get; set; }

        public DateTime? LastHit { get; set; }
        
        public string Subject { get; set; }

        public string LetterLink { get; set; }

        public string Note { get; set; }
    }
}
