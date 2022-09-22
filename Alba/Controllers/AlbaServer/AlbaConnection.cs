using Controllers.UseCases;
using Newtonsoft.Json;
using System;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AlbaConnection 
    {
        private const string SessionKeyName = "Alba3";

        IWebClient _webClient;
        Credentials _credentials;

        /// <summary>
        /// Connects to Alba and authorizes a user.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">The FQDN name where the Alba applicaton is hosted.  Example: www.my-alba-host.com</param>
        /// <param name="applicationPath">Example: "/alba"</param>
        public AlbaConnection(IWebClient webClient, ApplicationBasePath basePath)
        {
            _webClient = webClient;
            BasePath = basePath;
        }

        public ApplicationBasePath BasePath { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; private set; }
        public string AccountFullName { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string Province { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public bool IsAuthenticated { get; set; }

        public static AlbaConnection From(string site)
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: site,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }

        public void Authenticate(Credentials credentials)
        {
            _credentials = credentials;

            // We need to load the logon page first to get the session key
            string html = GetLogonPage();

            credentials.K1MagicString = ExtractAuthK1.ExtractFrom(html);
            credentials.SessionKeyValue = _webClient.GetCookieValue(SessionKeyName);

            //_webClient.AddCookie("alba_an", credentials.Account, BasePath.ApplicationPath, BasePath.Site);
            //_webClient.AddCookie("alba_us", credentials.User, BasePath.ApplicationPath, BasePath.Site);
            //_webClient.AddCookie(SessionKeyName, credentials.SessionKeyValue, "/", BasePath.Site);

            SubmitCredentials(_credentials);
        }

        private string GetLogonPage()
        {
            return DownloadString("/");
        }

        private void SubmitCredentials(Credentials credentials)
        {
            var result = DownloadString(RelativeUrlBuilder.AuthenticationUrlFrom(credentials));
            var logonResult = JsonConvert.DeserializeObject<LogonResult>(result);

            LogonResultChecker.CheckForErrors(logonResult);

            if(logonResult?.user == null)
            {
                return;
            }

            IsAuthenticated = true;

            var user = logonResult.user;

            // In the JSON returned the user.id is the Account ID
            AccountId = user.id ?? 0;
            AccountName = user.account_name;
            AccountFullName = user.account_full_name;
            Address = user.address;
            City = user.city;
            Province = user.province;
            Country = user.country;
            PostalCode = user.postcode;
            Latitude = user.location_lat ?? 0.0;
            Longitude = user.location_lng ?? 0.0;

            if (AccountId == 0)
            {
                throw new ArgumentException("Account ID cannot be zero");
            }
        }

        public string DownloadString(string url)
        {
            _webClient.AddCookie("alba_an", _credentials.Account, BasePath.ApplicationPath, BasePath.Site);
            _webClient.AddCookie("alba_us", _credentials.User, BasePath.ApplicationPath, BasePath.Site);
            _webClient.AddCookie(SessionKeyName, _credentials.SessionKeyValue, "/", BasePath.Site);
   
            return _webClient.DownloadString(BasePath.BaseUrl + url);
        }
    }
}
