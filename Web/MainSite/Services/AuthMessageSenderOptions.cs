namespace WebUI.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string AuthEmailFromEmailAddress { get; set; }
        public string AuthEmailFromName { get; set; }
        public string AuthEmailTo { get; set; }
        public string PurchaseManager { get; set; }
        public string BuyerEmail { get; set; }
    }
}