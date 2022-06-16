using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaAuthClientService
    {
        AlbaConnection AuthClient();
        string DownloadString(string uri, string userName);
    }

    public class AlbaAuthClientService : IAlbaAuthClientService
    {
        readonly WebUIOptions _options;
        readonly IAlbaCredentialService _albaCredentialService;

        public AlbaAuthClientService(
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
        }

        public AlbaConnection AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: _options.AlbaHost,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }

        public string DownloadString(string uri, string userName)
        {
            Credentials credentials = _albaCredentialService.GetCredentialsFrom(userName);

            AlbaConnection client = AuthClient();

            client.Authenticate(credentials);

            return client.DownloadString(uri);
        }
    }
}
