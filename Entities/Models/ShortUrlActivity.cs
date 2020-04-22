using System;

namespace WebUI.Areas.UrlShortener.Models
{
    public class ShortUrlActivity
    {
        public int Id { get; set; }

        public int ShortUrlId { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}
