using AlbaClient.Models;

namespace AlbaClient.Nominatim
{
    public class NominatimClient 
    {
        private string baseUrl;
        private IWebClient webClient;

        /// <summary>
        /// Connects to Alba and authorizes a user.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">The DNS name where the Alba applicaton is hosted.  Example: www.baseloc.com</param>
        /// <param name="applicationPath">Example: "/alba"</param>
        public NominatimClient(IWebClient webClient, ApplicationBasePath basePath)
        {
            this.webClient = webClient;
            BasePath = basePath;
            baseUrl = basePath.BaseUrl;
        }

        public ApplicationBasePath BasePath;

        public void Authorize(Credentials credentials)
        {
            webClient = GetWebClientWithCookies(credentials);
        }

        private IWebClient GetWebClientWithCookies(Credentials credentials)
        {
            //webClient.AddCookie("alba_an", credentials.Account, BasePath.ApplicationPath, BasePath.Site);
            //webClient.AddCookie("alba_us", credentials.User, BasePath.ApplicationPath, BasePath.Site);

            return webClient;
        }

        public string DownloadString(string url)
        {
            return webClient.DownloadString(BasePath.BaseUrl + url);
        }
    }
}
