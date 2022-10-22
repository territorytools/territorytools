using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IPhoneTerritoryAddWriterService
    {
        AddWriterResult AddWriter(string documentId, string userId);
    }

    public class PhoneTerritoryAddWriterService : IPhoneTerritoryAddWriterService
    {
        private readonly ISheetExtractor _sheetExtractor;
        private readonly WebUIOptions _options;
        private readonly MainDbContext _mainDbContext;

        public PhoneTerritoryAddWriterService(
            ISheetExtractor sheetExtractor,
            IOptions<WebUIOptions> optionsAccessor,
            MainDbContext mainDbContext)
        {
            _sheetExtractor = sheetExtractor;
            _options = optionsAccessor.Value;
            _mainDbContext = mainDbContext;
        }

        public AddWriterResult AddWriter(string documentId, string userId)
        {
            var result = new AddWriterResult();
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                result.Message = $"Badly formatted userID {userId}.  Select a valid user.";
                return result;
            }

            var user = _mainDbContext.TerritoryUser.FirstOrDefault(u => u.Id == userGuid);

            if (user == null)
            {
                result.Message = $"No such userID {userId}";
                return result;
            }

            var request = new AddSheetWriterRequest()
            {
                DocumentId = documentId,
                UserEmail = user.Email,
                SecurityToken = System.IO.File.ReadAllText("./secrets/GoogleApi.secrets.json")
            };

            string uri = _sheetExtractor.AddSheetWriter(request);

            return new AddWriterResult
            {
                Success = true,
                Message = $"Successfully added writer: {user.Email}",
                Uri = uri
            };
        }
    }
}
