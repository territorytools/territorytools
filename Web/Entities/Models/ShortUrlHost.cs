using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Areas.UrlShortener.Models
{
    public class ShortUrlHost
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool AllowNewUrls { get; set; }

        public List<ShortUrl> Urls { get; set; }

        public DateTime Created { get; set; } = DateTime.MinValue;
    }
}
