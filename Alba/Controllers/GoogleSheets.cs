using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TerritoryTools.Alba.Controllers
{
    public interface ISheets
    {
        void Write(string documentId, string range, IList<IList<object>> values);
        IList<IList<object>> Read(string documentId, string range);
        Spreadsheet CreateSheet(string title);
    }

    public class GoogleSheets : ISheets
    {
        // Some APIs, like Storage, accept a credential in their Create()
        // method.
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly, SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Experiment";
        const string CredPath = "token.json";

        readonly SheetsService _service;

        public GoogleSheets(string json)
        {
            if (IsJsonForAServiceAccount(json))
            {
                _service = GetSheetUserService(ServiceCredentials(json));
            }
            else
            {
                _service = GetSheetUserService(UserCredentials(json));
            }
        }

        public void Write(string documentId, string range, IList<IList<object>> values)
        {
            var valueRange = new ValueRange() { Values = values };
            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                _service.Spreadsheets.Values.Update(valueRange, documentId, range);

            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            updateRequest.Execute();
        }

        public IList<IList<object>> Read(string documentId, string range)
        {
            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request =
                    _service.Spreadsheets.Values.Get(documentId, range);

                ValueRange response = request.Execute();

                return response.Values;
            }
            catch (Exception e)
            {
                throw new Exception($"Read Google Sheet Error! DocumentId: {documentId} Range: {range} Error Message: {e.Message}", e);
            }
        }

        public static bool IsJsonForAServiceAccount(string json)
        {
            JObject document = JObject.Parse(json);

            bool isServiceAccount;
            if ((string)document.SelectToken("type") == "service_account")
            {
                isServiceAccount = true;
            }
            // This file looks like an OAuth 2.0 JSON fileren.
            else if(document.SelectToken("installed.redirect_uris") != null)
            {
                isServiceAccount = false;
            }
            else
            {
                throw new Exception("Unknown secrets json file type");
            }

            return isServiceAccount;
        }

        public Spreadsheet CreateSheet(string title)
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            var properties = new SpreadsheetProperties()
            {
                Title = title,
            };
            spreadsheet.Properties = properties;
            Sheet sheet = new Sheet();
            var sheetProperties = new SheetProperties()
            {
                Title = "My Sheet"
            };
            sheet.Properties = sheetProperties;
            spreadsheet.Sheets = new List<Sheet>();
            spreadsheet.Sheets.Add(sheet);

            Sheet sheet2 = new Sheet();
            var sheetProperties2 = new SheetProperties()
            {
                Title = "Requests"
            };
            sheet2.Properties = sheetProperties2;
            spreadsheet.Sheets.Add(sheet2);

            var newSheetRequest = new SpreadsheetsResource.CreateRequest(_service, spreadsheet);
            SpreadsheetsResource.CreateRequest request = _service.Spreadsheets.Create(spreadsheet);
            Spreadsheet result = request.Execute();
            Console.WriteLine($"SpreadsheetID: {result.SpreadsheetId}");
            Console.WriteLine($"SpreadsheetUrl: {result.SpreadsheetUrl}");
            foreach(var s in result.Sheets)
            {
                Console.WriteLine($"{s.Properties.Title} ID: {s.Properties.SheetId}");
            }

            return result;
        }

        static SheetsService GetSheetUserService(ICredential credential)
        {
            //UserCredential credential = Credentials(jsonPath);

            // Create Google Sheets API service.
            SheetsService service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        static SheetsService GetSheetServiceService(string json)
        {
            GoogleCredential credential = ServiceCredentials(json);

            // Create Google Sheets API service.
            SheetsService service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        static UserCredential UserCredentials(string json)
        {
            UserCredential credential;

            byte[] byteArray = Encoding.ASCII.GetBytes(json);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.

                string path = Path.Combine(Path.GetTempPath(), CredPath);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(path, true)).Result;

                Console.WriteLine("Credential file saved to: " + CredPath);
            }

            return credential;
        }

        static GoogleCredential ServiceCredentials(string json)
        {
            GoogleCredential credential;

            byte[] byteArray = Encoding.ASCII.GetBytes(json);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                var serviceCred = ServiceAccountCredential.FromServiceAccountData(stream);
                credential = GoogleCredential.FromServiceAccountCredential(serviceCred);
            }

            return credential;
        }
    }
}