using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IPhoneTerritoryAssignmentService
    {
        GetAllAssignmentsResult GetAllPhoneAssignments();
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

        public PhoneTerritoryAssignmentService(
            IPhoneTerritoryAssignmentGateway phoneTerritoryAssignmentGateway, 
            IMemoryCache memoryCache)
        {
            _phoneTerritoryAssignmentGateway = phoneTerritoryAssignmentGateway;
            _memoryCache = memoryCache;
        }

        public GetAllAssignmentsResult GetAllPhoneAssignments()
        {
            if (!_memoryCache.TryGetValue(
                    "AllPhoneTerritoryAssignments",
                    out GetAllAssignmentsResult cacheValue))
            {
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

                return returnResult;
            }

            return cacheValue;
        }
    }
}
