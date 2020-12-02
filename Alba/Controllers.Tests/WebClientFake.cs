using TerritoryTools.Alba.Controllers;

namespace AlbaClient.Tests
{
    public class WebClientFake : IWebClient
    {
        public void AddCookie(string name, string value, string path, string domain)
        {
        }

        public string DownloadStringReturns;
        public string GetCookieValueReturns;

        public string DownloadString(string url)
        {
            return DownloadStringReturns;
        }

        public string GetCookieValue(string name)
        {
            return GetCookieValueReturns;
        }
    }
}
