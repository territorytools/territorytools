using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public interface IAlbaAuthClientService
    {
        AlbaConnection AuthClient();
        string DownloadString(string uri, Credentials credentials);
    }

    public class AlbaAuthClientService : IAlbaAuthClientService
    {
        private readonly Credentials _credentials;

        public AlbaAuthClientService(Credentials credentials)
        {
            _credentials = credentials;
        }

        public AlbaConnection AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: _credentials.Host,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }

        public string DownloadString(string uri, Credentials credentials)
        {
            AlbaConnection client = AuthClient();

            if (!client.IsAuthenticated)
            {
                client.Authenticate(credentials);
            }

            return client.DownloadString(uri);
        }
    }
}
