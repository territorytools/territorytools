using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TerritoryTools.Web.MainSite
{
    // There is another copy of this in TerritoryTools.Mobile/TerritoryTools.Mobile
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage targetRequestMessage,
            HttpCompletionOption option,
            CancellationToken cancellationToken);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage targetRequestMessage, 
            HttpCompletionOption option, 
            CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(targetRequestMessage, option, cancellationToken);
        }
    }
}
