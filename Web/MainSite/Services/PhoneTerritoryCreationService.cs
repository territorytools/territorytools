using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IPhoneTerritoryCreationService
    {
        PhoneTerritoryCreateResult CreateTerritory(
           string sourceDocumentId,
           string sourceSheetName,
           string territoryNumber,
           string userId,
           string assignerEmail,
           string assigneeEmail,
            string assigneeFullName);
    }

    public class PhoneTerritoryCreationService : IPhoneTerritoryCreationService
    {
        private readonly ISheetExtractor _sheetExtractor;
        private readonly WebUIOptions _options;
        private readonly MainDbContext _mainDbContext;

        public PhoneTerritoryCreationService(
            ISheetExtractor sheetExtractor,
            IOptions<WebUIOptions> optionsAccessor, 
            MainDbContext mainDbContext)
        {
            _sheetExtractor = sheetExtractor;
            _options = optionsAccessor.Value;
            _mainDbContext = mainDbContext;
        }

        public PhoneTerritoryCreateResult CreateTerritory(
            string sourceDocumentId,
            string sourceSheetName,
            string territoryNumber,
            string userId,
            string assignerEmail,
            string assigneeEmail,
            string assigneeFullName)
        {
            var result = new PhoneTerritoryCreateResult();

            string userEmail = string.Empty;
            string userFullName = string.Empty;

            bool userIdIsGuid = Guid.TryParse(userId, out Guid userGuid);

            if (userId == "SHARED" || assigneeEmail == "SHARED")
            {
                userEmail = _options.SharedPhoneTerritoryEmailAddress;
                userFullName = _options.SharedPhoneTerritoryFullName;
            }
            else if(string.IsNullOrEmpty(assigneeEmail) && !userIdIsGuid)
            {
                result.Message = $"Assignee Email is missing";
                return result;
            }
            else if (string.IsNullOrEmpty(assigneeEmail) && userIdIsGuid)
            {
                result.Message = $"Bad userId, not a GUID";
                return result;
            }
            else if(userIdIsGuid)
            {
                Entities.TerritoryUser? user = _mainDbContext.TerritoryUser.FirstOrDefault(u => u.Id == userGuid);
                if (user == null)
                {
                    result.Message = $"No such userID {userId}";
                    return result;
                }
                else
                {
                    userEmail = user.Email;
                    userFullName = $"{user.GivenName} {user.Surname}".Trim();
                }
            }
            else
            {
                userEmail = assigneeEmail;
                userFullName = assigneeFullName;
            }

            var request = new SheetExtractionRequest()
            {
                FromDocumentId = sourceDocumentId,
                PublisherEmail = userEmail,
                FromSheetName = sourceSheetName,
                OwnerEmail = userEmail,
                PublisherName = userFullName,
                TerritoryNumber = territoryNumber,
                SecurityToken = System.IO.File.ReadAllText("./secrets/GoogleApi.secrets.json")
            };

            SheetExtractionResult extractionResult = _sheetExtractor.Extract(request);
            string uri = $"https://docs.google.com/spreadsheets/d/{extractionResult.DocumentId}";

            _sheetExtractor.AddSheetReader(new AddSheetWriterRequest
            {
                DocumentId = sourceDocumentId,
                SecurityToken = System.IO.File.ReadAllText("./secrets/GoogleApi.secrets.json"),
                UserEmail = assignerEmail,
            });

            result.Success = true;
            result.Message = $"Successfully created and assigned to {userEmail.ToLower()}";
            result.Item = new PhoneTerritoryCreateItem
                {
                    Description = $"Territory {extractionResult.TerritoryNumber}",
                    Uri = uri,
                    DocumentId = extractionResult.DocumentId
            };

            return result;
        }
    }
}
