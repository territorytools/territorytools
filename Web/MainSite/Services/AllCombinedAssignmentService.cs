using Controllers.UseCases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ICombinedAssignmentService
    {
        GetAllAssignmentsResult GetAllAssignments(string userName);
        void LoadAssignments(string userName);
    }

    public class AllCombinedAssignmentService : ICombinedAssignmentService
    {
        private readonly IAlbaAssignmentGateway _albaAssignmentGateway;
        private readonly IPhoneTerritoryAssignmentService _phoneTerritoryAssignmentService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AllCombinedAssignmentService> _logger;

        public AllCombinedAssignmentService(
            IAlbaAssignmentGateway albaAssignmentGateway,
            IPhoneTerritoryAssignmentService phoneTerritoryAssignmentService,
            IMemoryCache memoryCache,
            ILogger<AllCombinedAssignmentService> logger)
        {
            _albaAssignmentGateway = albaAssignmentGateway;
            _phoneTerritoryAssignmentService = phoneTerritoryAssignmentService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public GetAllAssignmentsResult GetAllAssignments(string userName)
        {
            var allAssignments = new List<AlbaAssignmentValues>();

            var result = _albaAssignmentGateway.GetAlbaAssignments(userName);
            if (result.Success)
            {
                allAssignments.AddRange(result.AssignmentValues);
            }
            else
            {
                var backUpValues = LoadCsv<TerritoryTools.Alba.Controllers.AlbaServer.AlbaAssignment>
                    .LoadFrom("./territories.txt");

                foreach (var backUpValue in backUpValues)
                {
                    allAssignments.Add(
                        new AlbaAssignmentValues
                        {
                            Description = backUpValue.Description,
                            Id = backUpValue.Id,
                            IdString = backUpValue.Id.ToString(),
                            SignedOutTo = backUpValue.SignedOutTo,
                            SignedOut = backUpValue.SignedOut,
                            MobileLink = backUpValue.MobileLink,
                            Status = backUpValue.Status,
                            Number = backUpValue.Number,
                            Kind = backUpValue.Kind,
                            LastCompletedBy = backUpValue.LastCompletedBy,
                            LastCompleted = backUpValue.LastCompleted,

                        });
                }

                _logger.LogInformation($"Loaded, but not cached, {allAssignments.Count} assignments from back-up file on behalf of userName: {userName}");
            }

            var phoneTerritories = _phoneTerritoryAssignmentService.GetAllPhoneAssignments();
            allAssignments.AddRange(phoneTerritories.Rows);

            return new GetAllAssignmentsResult
            {
                PhoneSuccess = phoneTerritories.PhoneSuccess,
                Rows = allAssignments
            };
        }

        public void LoadAssignments(string userName)
        {
            _albaAssignmentGateway.LoadAlbaAssignments(userName);
            _phoneTerritoryAssignmentService.LoadAssignments();
        }
    }
}
