namespace TerritoryTools.Web.MainSite.Models
{
    public class AddWriterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Uri { get; set; } = string.Empty;
    }
}