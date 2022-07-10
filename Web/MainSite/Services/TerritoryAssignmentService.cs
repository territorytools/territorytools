using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoryAssignmentService
    {
        IEnumerable<AlbaAssignmentValues> NeverCompleted(string userName);
        IEnumerable<Publisher> ByPublisher(string userName);
        void Assign(int territoryId, int userId, string user);
        void Unassign(int territoryId, string userName);
        void Complete(int territoryId, string userName);
    }

    public class TerritoryAssignmentService : ITerritoryAssignmentService
    {
        readonly ICombinedAssignmentService _combinedAssignmentService;
        readonly IUserService _userService;
        readonly IAlbaAuthClientService _albaAuthClientService;
        readonly ILogger<TerritoryAssignmentService> _logger;
        readonly WebUIOptions options;

        public TerritoryAssignmentService(
            ICombinedAssignmentService combinedAssignmentService,
            IUserService userService,
            IAlbaAuthClientService albaAuthClientService,
            IOptions<WebUIOptions> optionsAccessor,
            ILogger<TerritoryAssignmentService> logger)
        {
            _combinedAssignmentService = combinedAssignmentService;
            _userService = userService;
            _albaAuthClientService = albaAuthClientService;
            _logger = logger;
            options = optionsAccessor.Value;
        }

        public IEnumerable<AlbaAssignmentValues> NeverCompleted(string userName)
        {
            try
            {
                return _combinedAssignmentService.GetAllAssignments(userName)
                    // Territories never worked
                    .Rows
                    .Where(a => a.LastCompleted == null && a.SignedOut == null)
                    .OrderBy(a => a.Description)
                    .ThenBy(a => a.Number);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                throw;
            }
        }

        public IEnumerable<Publisher> ByPublisher(string userName)
        {
            try
            {
                var groups = _combinedAssignmentService.GetAllAssignments(userName)
                    .Rows
                    .Where(a => !string.IsNullOrWhiteSpace(a.SignedOutTo))
                    .GroupBy(a => a.SignedOutTo)
                    .ToList();

                var publishers = new List<Publisher>();
                foreach (var group in groups.OrderBy(g => g.Key))
                {
                    var pub = new Publisher() { Name = group.Key };
                    foreach (var item in group.OrderByDescending(a => a.SignedOut))
                    {
                        pub.Territories.Add(item);
                    }

                    publishers.Add(pub);
                }

                return publishers;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                throw;
            }
        }

        public void Assign(int territoryId, int userId, string user)
        {
            _logger.LogInformation($"Assigning territoryId {territoryId} to userId: {userId} ({user})");

            string result = _albaAuthClientService.DownloadString(
                RelativeUrlBuilder.AssignTerritory(
                    territoryId,
                    userId,
                    DateTime.Now),
                user);

            global::Controllers.UseCases.User myUser = _userService.GetUsers(user)
                .FirstOrDefault(u => u.Id == userId);

            string userName = "Somebody";
            if (myUser != null)
            {
                userName = myUser.Name;
            }

            // This should refresh the mobile territory link to send to the user
            _combinedAssignmentService.LoadAssignments(user);
        }

        public void Unassign(int territoryId, string userName)
        {
            _logger.LogInformation($"Unassigning territoryId {territoryId} ({userName})");

            _albaAuthClientService.DownloadString(
                RelativeUrlBuilder.UnassignTerritory(territoryId),
                userName);

            _combinedAssignmentService.LoadAssignments(userName);
        }

        public void Complete(int territoryId, string userName)
        {
            _logger.LogInformation($"Marking as complete territoryId {territoryId} ({userName})");

            _albaAuthClientService.DownloadString(
                RelativeUrlBuilder.SetTerritoryCompleted(territoryId, DateTime.Today), 
                userName);

            _combinedAssignmentService.LoadAssignments(userName);
        }
    }

    public class Publisher
    {
        public string Name { get; set; }
        public List<AlbaAssignmentValues> Territories { get; set; } = new List<AlbaAssignmentValues>();
    }
}
