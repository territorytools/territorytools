namespace AlbaClient.Tests
{
    public class WebClientFake : IWebClient
    {
        public void AddCookie(string name, string value, string path, string domain)
        {
        }

        public string DownloadStringReturns;

        public string DownloadString(string url)
        {
            return DownloadStringReturns;
        }
    }
}
