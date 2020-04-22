using System.ComponentModel.DataAnnotations;

namespace WebUI.Areas.UrlShortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        
        [Required]
        public string OriginalUrl { get; set; }

        public string UserName { get; set; }
        
        public string Subject { get; set; }
        
        public string Note { get; set; }
    }
}
