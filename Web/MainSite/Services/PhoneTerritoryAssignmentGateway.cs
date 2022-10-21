using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.PhoneTerritorySheets;
using TerritoryTools.Web.Data;
using TerritoryTools.Web.MainSite.Models;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IPhoneTerritoryAssignmentGateway
    {
        PhoneTerritoryAssignmentServiceGetAllResult GetAllAssignments();
    }

    public class PhoneTerritoryAssignment
    {
        public string TerritoryNumber { get; set; }
        public string Date { get; set; }
        public string Publisher { get; set; }
        public string Transaction { get; set; }
        public string Notes { get; set; }
        public string SheetLink { get; set; }
        public string CheckedOut { get; set; }
        public string CheckedIn { get; set; }
    }

    public class PhoneTerritoryAssignmentGateway : IPhoneTerritoryAssignmentGateway
    {
        readonly ISpreadSheetService _spreadSheetService;
        readonly ISheetExtractor _sheetExtractor;
        readonly ITerritoryUserService _territoryUserService;
        readonly WebUIOptions _options;
        readonly MainDbContext _mainDbContext;
        readonly ILogger<PhoneTerritoryAssignmentGateway> _logger;
        string _documentId = string.Empty;
        string _sheetName = string.Empty;

        public PhoneTerritoryAssignmentGateway(
            ISpreadSheetService spreadSheetService,
            ISheetExtractor sheetExtractor,
            ITerritoryUserService territoryUserService,
            IOptions<WebUIOptions> optionsAccessor,
            MainDbContext mainDbContext,
            ILogger<PhoneTerritoryAssignmentGateway> logger)
        {
            _spreadSheetService = spreadSheetService;
            _sheetExtractor = sheetExtractor;
            _territoryUserService = territoryUserService;
            _options = optionsAccessor.Value;
            _mainDbContext = mainDbContext;
            _logger = logger;
            _documentId = _options.DefaultPhoneTerritorySourceDocumentId;
            _sheetName = _options.DefaultPhoneTerritorySourceSheetName;
        }

        public PhoneTerritoryAssignmentServiceGetAllResult GetAllAssignments()
        {
            try
            {
                _logger.LogInformation($"Getting all assignments from phone territory spreadsheet documentId: {_documentId} sheetName: {_sheetName}");

                IList<IList<object>> values = _spreadSheetService.Read(_documentId, "Assignments");
                if (values.Count == 0)
                    throw new Exception($"Empty sheet: Spreadsheet ID {_documentId} Sheet Name {_sheetName}");

                if (values[0].Count == 0)
                    throw new Exception($"Empty top row Spreadsheet ID {_documentId} Sheet Name {_sheetName}");

                if ((values[0][0] as string) != "Number"
                    || (values[0][1] as string) != "Date"
                    || (values[0][2] as string) != "Transaction"
                    || (values[0][3] as string) != "Publisher"
                    || (values[0][4] as string) != "Note"
                    || (values[0][5] as string) != "Link to Document"
                    || (values[0][6] as string) != "Checked Out"
                    || (values[0][7] as string) != "Checked In")
                    throw new Exception($"Top row is not a valid header row!");

                var allPhoneRows = new List<PhoneTerritoryAssignment>();
                // Start with the second row
                for (int i = 1; i < values.Count; i++)
                {
                    IList<object> row = values[i];
                    allPhoneRows.Add(new PhoneTerritoryAssignment
                    {
                        TerritoryNumber = row.Count > 0 ? row[0] as string : null,
                        Date = row.Count > 1 ? row[1] as string : null,
                        Transaction = row.Count > 2 ? row[2] as string : null,
                        Publisher = row.Count > 3 ? row[3] as string : null,
                        Notes = row.Count > 4 ? row[4] as string : null,
                        SheetLink = row.Count > 5 ? row[5] as string : null,
                        CheckedOut = row.Count > 6 ? row[6] as string : null,
                        CheckedIn = row.Count > 7 ? row[7] as string : null,
                    });
                }

                return new PhoneTerritoryAssignmentServiceGetAllResult
                {
                    Success = true,
                    Rows = allPhoneRows
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error get phone assignments from Google Spreadsheet: documentId: {_documentId} sheetName: {_sheetName} Error: {ex}");
                return new PhoneTerritoryAssignmentServiceGetAllResult
                {
                    Success = false,
                    Rows = new List<PhoneTerritoryAssignment>()
                };
            }
        }

        public AddWriterResult AddWriter(string documentId, string userId)
        {
            _logger.LogInformation($"Adding writer to phone territory spreadsheet documentId: {documentId} userId: {userId}");

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
