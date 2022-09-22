using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IUserFromApiService
    {
        UserContract ByFullName(string userFullName);
        UserContract ByEmail(string userEmail);
        int Add(UserContract user);
    }

    public class UserFromApiService : IUserFromApiService
    {
        private readonly IApiService _apiService;
        readonly ILogger<AssignLatestService> _logger;
        private readonly IConfiguration _configuration;
        readonly WebUIOptions _options;

        public UserFromApiService(
            IApiService apiService,
            ILogger<AssignLatestService> logger,
            IOptions<WebUIOptions> optionsAccessor,
            IConfiguration configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration;
            _options = optionsAccessor.Value;
        }

        public UserContract ByFullName(string userFullName)
        {
            return _apiService.Get<UserContract>("users", $"?userFullName={userFullName}");
        }

        public UserContract ByEmail(string userEmail)
        {
            return _apiService.Get<UserContract>("users/single", $"?email={userEmail}");
        }

        public int Add(UserContract user)
        {
            return _apiService.Post<int, UserContract>(
                relativePath: "users",
                queryString: $"?email={user.NormalizedEmail}", 
                body: user);
        }
    }
}