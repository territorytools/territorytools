using System;

namespace WebUI
{
    public class WebUIOptions
    {
        public string AlbaUserManagementHtmlPath { get; set; }
            = "/data/user-management.html";
        public string AlbaUsersHtmlPath { get; set; } 
            = "/data/users.html";
        public string AlbaAssignmentsHtmlPath { get; set; }
            = "/data/assignments.html";
    }
}