namespace TerritoryTools.Web.MainSite.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string AuthEmailFromEmailAddress { get; set; }
        public string AuthEmailFromName { get; set; }
        public string AuthEmailTo { get; set; }
    }
}