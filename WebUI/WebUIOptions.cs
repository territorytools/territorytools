using System;

namespace WebUI
{
    public class WebUIOptions
    {
        public string AlbaUserManagementHtmlPath { get; set; }
            = "/data/user-management.html";
        public string AlbaUsersHtmlPath { get; set; } 
            = "/data/alba/accounts/{0}/users.html";
        public string AlbaAssignmentsHtmlPath { get; set; }
            = "/data/assignments.html";
        public string AzureAppId { get; set; }
        public string AzureClientSecret { get; set; }
        public string UrlShortenerDomain { get; set; }
    }
}