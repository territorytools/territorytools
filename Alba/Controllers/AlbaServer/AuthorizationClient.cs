using TerritoryTools.Alba.Controllers.Models;
using Controllers.UseCases;
using Newtonsoft.Json;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AuthorizationClient 
    {
        private const string SessionKeyName = "Alba3";

        private IWebClient webClient;

        /// <summary>
        /// Connects to Alba and authorizes a user.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">The FQDN name where the Alba applicaton is hosted.  Example: www.my-alba-host.com</param>
        /// <param name="applicationPath">Example: "/alba"</param>
        public AuthorizationClient(IWebClient webClient, ApplicationBasePath basePath)
        {
            this.webClient = webClient;
            BasePath = basePath;
        }

        public ApplicationBasePath BasePath { get; set; }
        public int AccountId { get; set; }

        public void Authenticate(Credentials credentials)
        {
            // We need to load the logon page first to get the session key
            string html = GetLogonPage();

            credentials.K1MagicString = ExtractAuthK1.ExtractFrom(html);
            credentials.SessionKeyValue = webClient.GetCookieValue(SessionKeyName);

            webClient.AddCookie("alba_an", credentials.Account, BasePath.ApplicationPath, BasePath.Site);
            webClient.AddCookie("alba_us", credentials.User, BasePath.ApplicationPath, BasePath.Site);
            webClient.AddCookie(SessionKeyName, credentials.SessionKeyValue, "/", BasePath.Site);

            SubmitCredentials(credentials);
        }

        private string GetLogonPage()
        {
            return DownloadString("/");
        }

        private void SubmitCredentials(Credentials credentials)
        {
            var result = DownloadString(RelativeUrlBuilder.AuthenticationUrlFrom(credentials));
            var logonResult = JsonConvert.DeserializeObject<LogonResult>(result);

            AccountId =  logonResult?.user?.id ?? 0;

            LogonResultChecker.CheckForErrors(logonResult);
        }

        public string DownloadString(string url)
        {
            return webClient.DownloadString(BasePath.BaseUrl + url);
        }
    }
}
