using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace TerritoryTools.Web.MainSite.Services
{
    public class DatabaseInfo
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseServer { get; set; } = string.Empty;
        public string DatabaseUser { get; set; } = string.Empty;
    }

    public interface IDatabaseInfoService
    {
        DatabaseInfo Info();
    }

    public class DatabaseInfoService : IDatabaseInfoService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseInfoService> _logger;

        public DatabaseInfoService(IConfiguration configuration, ILogger<DatabaseInfoService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public DatabaseInfo Info()
        {
            DatabaseInfo info = new();

            string connectionString = _configuration.GetConnectionString("MainDbContextConnection");

            if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains(";"))
            {
                string[] parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
                string? server = parts
                    .FirstOrDefault(p => p.StartsWith("Server=", StringComparison.OrdinalIgnoreCase))?
                    .Replace("Server=", "", StringComparison.OrdinalIgnoreCase);

                string? database = parts
                    .FirstOrDefault(p =>
                           p.StartsWith("Database=", StringComparison.OrdinalIgnoreCase)
                        || p.StartsWith("Initial Catalog=", StringComparison.OrdinalIgnoreCase))?
                    .Replace("Database=", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Initial Catalog=", "", StringComparison.OrdinalIgnoreCase);

                string? user = parts
                    .FirstOrDefault(p =>
                           p.StartsWith("User ID=", StringComparison.OrdinalIgnoreCase)
                        || p.StartsWith("User=", StringComparison.OrdinalIgnoreCase))?
                    .Replace("User ID=", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("User=", "", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation(
                    $"Database Server: {server}\n" +
                    $"      Database Name  : {database}\n" +
                    $"      Database User  : {user}");

                info.DatabaseServer = server ?? "null";
                info.DatabaseName = database ?? "null";
                info.DatabaseUser = user ?? "null";
            }
            else
            {
                _logger.LogError("Not database connection string found!");
            }

            return info;
        }
    }

}
