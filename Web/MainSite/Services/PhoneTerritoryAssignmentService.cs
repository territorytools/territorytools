using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IPhoneTerritoryAssignmentService
    {
        GetAllAssignmentsResult GetAllPhoneAssignments();
        void LoadAssignments();
    }

    public class GetAllAssignmentsResult
    {
        public bool PhoneSuccess { get; set; }
        public IEnumerable<AlbaAssignmentValues> Rows { get; set; }
    }

    public class PhoneTerritoryAssignmentService : IPhoneTerritoryAssignmentService
    {
        private readonly IPhoneTerritoryAssignmentGateway _phoneTerritoryAssignmentGateway;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PhoneTerritoryAssignmentService> _logger;

        public PhoneTerritoryAssignmentService(
            IPhoneTerritoryAssignmentGateway phoneTerritoryAssignmentGateway, 
            IMemoryCache memoryCache,
            ILogger<PhoneTerritoryAssignmentService> logger)
        {
            _phoneTerritoryAssignmentGateway = phoneTerritoryAssignmentGateway;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public GetAllAssignmentsResult GetAllPhoneAssignments()
        {
            if (!_memoryCache.TryGetValue(
                    "AllPhoneTerritoryAssignments",
                    out GetAllAssignmentsResult cacheValue))
            {
                GetAllAssignmentsResult returnResult = DownloadAssignments();

                return returnResult;
            }

            _logger.LogInformation($"Getting all {cacheValue.Rows.Count()} phone assignments from cache");

            return cacheValue;
        }

        public void LoadAssignments()
        {
            DownloadAssignments();
        }

        GetAllAssignmentsResult DownloadAssignments()
        {
            _logger.LogInformation($"Converting phone assignments from Google Sheet values");

            var allAssignments = new List<AlbaAssignmentValues>();
            var result = _phoneTerritoryAssignmentGateway.GetAllAssignments();

            foreach (var phoneAssignment in result.Rows)
            {
                DateTime.TryParse(phoneAssignment.Date, out DateTime date);
                var assignment = new AlbaAssignmentValues
                {
                    Number = phoneAssignment.TerritoryNumber,
                    SignedOutTo = phoneAssignment.Transaction == "Checked Out" ? phoneAssignment.Publisher : null,
                    SignedOut = phoneAssignment.Transaction == "Checked Out" ? date : null,
                    LastCompletedBy = phoneAssignment.Transaction == "Checked In" ? phoneAssignment.Publisher : null,
                    LastCompleted = phoneAssignment.Transaction == "Checked In" ? date : null,
                    Status = phoneAssignment.Transaction,
                    Description = "PHONE",
                    MonthsAgoCompleted = 0,
                    MobileLink = phoneAssignment.SheetLink
                };

                allAssignments.Add(assignment);
            }

            var returnResult = new GetAllAssignmentsResult
            {
                PhoneSuccess = result.Success,
                Rows = allAssignments
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15));

            _memoryCache.Set("AllPhoneTerritoryAssignments", returnResult, cacheEntryOptions);

            _logger.LogInformation($"{allAssignments.Count} phone assignments were cached");

            return returnResult;
        }
    }
}
