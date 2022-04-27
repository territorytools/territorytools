namespace TerritoryTools.Web.MainSite
{
    public class WebUIOptions
    {
        // The FQDN of the alba host, example: alba-host.org
        public string AlbaHost { get; set; }
        public string AlbaUserManagementHtmlPath { get; set; }
            = "/data/alba/accounts/{0}/user-management.html";
        public string AlbaUsersHtmlPath { get; set; } 
            = "/data/alba/accounts/{0}/users.html";
        public string AlbaAssignmentsHtmlPath { get; set; }
            = "/data/alba/accounts/{0}/assignments.html";
        public string AzureAppId { get; set; }
        public string AzureClientSecret { get; set; }
        public string UrlShortenerDomain { get; set; }
        public string CompletionMapUrl { get; set; }
        public string Users { get; set; }
        public string AdminUsers { get; set; }
        public string DefaultPhoneTerritorySourceDocumentId { get; set; }
        public string DefaultPhoneTerritorySourceSheetName { get; set; }
        public string SharedPhoneTerritoryEmailAddress { get; set; }
        public string SharedPhoneTerritoryFullName { get; set; }
        public string SmsApiUserName { get; set; }
        public string SmsApiPassword { get; set; }
        public string SmsFromPhoneNumber { get; set; }
        public string SmsMessageLogDocumentId { get; set; }
        public string SmsMessageLogSheetName { get; set; }
    }
}