using AlbaClient.Models;
using System;

namespace AlbaClient.AzureMaps
{
    public class AzureMapsClient 
    {
        private string baseUrl;
        private string subscriptionKey;
        private IWebClient webClient;

        /// <summary>
        /// Connects to Alba and authorizes a user.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">The DNS name where the Alba applicaton is hosted.  Example: www.baseloc.com</param>
        /// <param name="applicationPath">Example: "/alba"</param>
        public AzureMapsClient(
            IWebClient webClient, 
            ApplicationBasePath basePath, 
            string subscriptionKey)
        {
            if(string.IsNullOrWhiteSpace(subscriptionKey))
            {
                throw new ArgumentNullException(nameof(subscriptionKey));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            this.webClient = webClient;
            BasePath = basePath;
            baseUrl = basePath.BaseUrl;
            this.subscriptionKey = subscriptionKey;
        }

        public ApplicationBasePath BasePath;

        public void Authorize(Credentials credentials)
        {
            webClient = GetWebClientWithCookies(credentials);
        }

        private IWebClient GetWebClientWithCookies(Credentials credentials)
        {
            return webClient;
        }

        public string DownloadString(string url)
        {
            return webClient.DownloadString($"{BasePath.BaseUrl}{url}&api-version=1.0&subscription-key={subscriptionKey}");
        }
    }
}
