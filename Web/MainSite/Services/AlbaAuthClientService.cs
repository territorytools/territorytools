using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAlbaAuthClientService
    {
        AlbaConnection AuthClient();
    }

    public class AlbaAuthClientService : IAlbaAuthClientService
    {
        readonly WebUIOptions options;
        public AlbaAuthClientService(IOptions<WebUIOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
        }

        public AlbaConnection AuthClient()
        {
            var webClient = new CookieWebClient();
            var basePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: options.AlbaHost,
                applicationPath: "/alba");

            var client = new AlbaConnection(
                webClient: webClient,
                basePath: basePath);

            return client;
        }
    }
}
