using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TerritoryTools.Web.MainSite.Services;

namespace TerritoryTools.Web.MainSite
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer = null!;
        private readonly int _intervalSeconds = 3600;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAlbaCredentialService _credentialService;
        private readonly IConfiguration Configuration;
        private readonly IMemoryCache _memoryCache;
        readonly WebUIOptions options;
        //private readonly IServiceScopeFactory scopeFactory;

        public TimedHostedService(
            IServiceScopeFactory scopeFactory
            //IAlbaCredentialService credentialService, 
            //IConfiguration configuration,
            //ILogger<TimedHostedService> logger, 
            //IMemoryCache memoryCache,
            //IOptions<WebUIOptions> optionsAccessor
            )
        {
            _scopeFactory = scopeFactory;
            //_credentialService = credentialService;
            //Configuration = configuration;
            using var scope = _scopeFactory.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            _intervalSeconds = int.Parse(configuration["TimerIntervalSeconds"]);
            //_logger = logger;
            //_memoryCache = memoryCache;
            //options = optionsAccessor.Value;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TimedHostedService>>();

            logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_intervalSeconds));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            using var scope = _scopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TimedHostedService>>();

            logger.LogInformation($"Timed Hosted Service is working. Interval (sec): {_intervalSeconds} Count: {count}");

            //Guid albaAccountId = _credentialService
            //    .GetAlbaAccountIdFor(User.Identity.Name);

            //Load(albaAccountId);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TimedHostedService>>();

            logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}