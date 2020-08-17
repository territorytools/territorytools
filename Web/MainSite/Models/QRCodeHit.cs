namespace TerritoryTools.Web.MainSite
{
    public class QRCodeHit
    {
        public int Id { get; set; }
        public string ShortUrl { get; set; }
        public string OriginalUrl { get; set; }
        public string Created { get; set; }
        public string HitCount { get; set; }
        public string LastIPAddress { get; set; }
        public string LastTimeStamp { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
    }
}
