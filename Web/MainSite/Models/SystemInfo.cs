using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class SystemInfo
    {
        public string? ApiDatabaseName { get; set; }
        public string? ApiGitCommit { get; set; }
        public DatabaseInfo DatabaseInfo { get; set; } = new();
        public string? MachineName { get; set; }
        public string AlbaHost { get; set; }
        public string AlbaUserManagementHtmlPath { get; set; }
        public string AlbaUsersHtmlPath { get; set; }
        public string AlbaAssignmentsHtmlPath { get; set; }
        public string AzureAppId { get; set; }
        public string UrlShortenerDomain { get; set; }
        public string CompletionMapUrl { get; set; }
        public string Users { get; set; }
        public string AdminUsers { get; set; }
        public string DefaultPhoneTerritorySourceDocumentId { get; set; }
        public string DefaultPhoneTerritorySourceSheetName { get; set; }
        public string SharedPhoneTerritoryEmailAddress { get; set; }
        public string SharedPhoneTerritoryFullName { get; set; }
        public string SmsApiUserName { get; set; }
        public string SmsFromPhoneNumber { get; set; }
        public string SmsMessageLogDocumentId { get; set; }
        public string SmsMessageLogSheetName { get; set; }
        public string SmsAdminRecipient { get; set; }
        public string TerritoryApiBaseUrl { get; set; }
        public Features Features { get; set; } = new Features();
    }
}