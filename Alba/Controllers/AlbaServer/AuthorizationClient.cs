﻿using AlbaClient.Models;

namespace AlbaClient.AlbaServer
{
    public class AuthorizationClient 
    {
        private string baseUrl;
        private IWebClient webClient;

        /// <summary>
        /// Connects to Alba and authorizes a user.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">The DNS name where the Alba applicaton is hosted.  Example: www.baseloc.com</param>
        /// <param name="applicationPath">Example: "/alba"</param>
        public AuthorizationClient(IWebClient webClient, ApplicationBasePath basePath)
        {
            this.webClient = webClient;
            BasePath = basePath;
            baseUrl = basePath.BaseUrl;
        }

        public ApplicationBasePath BasePath;

        public void Authorize(Credentials credentials)
        {
            webClient = GetWebClientWithCookies(credentials);

            // We need to load the logon page first to get this client recognized by the server.
            GetLogonPage(); // TODO: Get the k1 magic string from the login page

            SubmitCredentials(credentials);
        }

        private IWebClient GetWebClientWithCookies(Credentials credentials)
        {
            webClient.AddCookie("alba_an", credentials.Account, BasePath.ApplicationPath, BasePath.Site);
            webClient.AddCookie("alba_us", credentials.User, BasePath.ApplicationPath, BasePath.Site);

            return webClient;
        }

        private void GetLogonPage()
        {
            DownloadString("/");
        }

        private void SubmitCredentials(Credentials credentials)
        {
            var result = DownloadString(RelativeUrlBuilder.Authenticate(credentials));

            LogonResultChecker.CheckForErrors(result);
        }

        public string DownloadString(string url)
        {
            return webClient.DownloadString(BasePath.BaseUrl + url);
        }
    }
}