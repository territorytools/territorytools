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

        public AllCombinedAssignmentService(
            IAlbaAssignmentGateway albaAssignmentGateway,
            IPhoneTerritoryAssignmentService phoneTerritoryAssignmentService)
        {
            _albaAssignmentGateway = albaAssignmentGateway;
            _phoneTerritoryAssignmentService = phoneTerritoryAssignmentService;
        }

        public GetAllAssignmentsResult GetAllAssignments(string userName)
        {
            var allAssignments = new List<AlbaAssignmentValues>();

            allAssignments.AddRange(_albaAssignmentGateway.GetAlbaAssignments(userName));

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
