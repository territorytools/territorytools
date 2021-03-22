using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Web.MainSite.Areas.UrlShortener.Models
{
    public class ShortUrlUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        public string Subject { get; set; }

        public string LetterLink { get; set; }

        public string Note { get; set; }
    }
}
