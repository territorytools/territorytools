//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Services;
//using Google.Apis.Sheets.v4;
//using Google.Apis.Sheets.v4.Data;
//using Google.Apis.Util.Store;
//using System.Text;
//using System.Text.Json;

//namespace TerritoryTools.Alba.Controllers
//{
//    public interface ISheets
//    {
//        void Write(string documentId, string range, IList<IList<object>> values);
//        IList<IList<object>> Read(string documentId, string range);
//    }

//    public class GoogleSheets : ISheets
//    {
//        // Some APIs, like Storage, accept a credential in their Create()
//        // method.
//        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly, SheetsService.Scope.Spreadsheets };
//        static string ApplicationName = "Google Sheets API .NET Experiment";
//        const string CredPath = "token.json";

//        readonly SheetsService _service;

//        public GoogleSheets(string json)
//        {
//            if (IsJsonForAServiceAccount(json))
//            {
//                _service = GetSheetUserService(ServiceCredentials(json));
//            }
//            else
//            {
//                _service = GetSheetUserService(UserCredentials(json));
//            }
//        }

//        public void Write(string documentId, string range, IList<IList<object>> values)
//        {
//            ValueRange valueRange = new() { Values = values };
//            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
//                _service.Spreadsheets.Values.Update(valueRange, documentId, range);

//            updateRequest.ValueInputOption =
//                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

//            updateRequest.Execute();
//        }

//        public IList<IList<object>> Read(string documentId, string range)
//        {
//            try
//            {
//                SpreadsheetsResource.ValuesResource.GetRequest request =
//                    _service.Spreadsheets.Values.Get(documentId, range);

//                ValueRange response = request.Execute();

//                return response.Values;
//            }
//            catch (Exception e)
//            {
//                throw new System.Exception($"Read Google Sheet Error! DocumentId: {documentId} Range: {range} Error Message: {e.Message}", e);
//            }
//        }

//        public static bool IsJsonForAServiceAccount(string? json)
//        {
//            var options = new JsonDocumentOptions
//            {
//                AllowTrailingCommas = true,
//                MaxDepth = 3
//            };

//            JsonDocument document = JsonDocument.Parse(json, options);

//            bool isServiceAccount;
//            if (document.RootElement.TryGetProperty("type", out JsonElement element)
//                && element.GetString() == "service_account")
//            {
//                isServiceAccount = true;
//            }

//            // This file looks like an OAuth 2.0 JSON file
//            else if (document.RootElement.TryGetProperty("installed", out JsonElement installedElement)
//                    && installedElement.TryGetProperty("redirect_uris", out _))
//            {
//                isServiceAccount = false;
//            }
//            else
//            {
//                throw new Exception("Unknown secrets json file type");
//            }

//            return isServiceAccount;
//        }

//        static SheetsService GetSheetUserService(ICredential credential)
//        {
//            //UserCredential credential = Credentials(jsonPath);

//            // Create Google Sheets API service.
//            SheetsService service = new(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = ApplicationName,
//            });

//            return service;
//        }

//        static SheetsService GetSheetServiceService(string json)
//        {
//            GoogleCredential credential = ServiceCredentials(json);

//            // Create Google Sheets API service.
//            SheetsService service = new(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = ApplicationName,
//            });

//            return service;
//        }

//        static UserCredential UserCredentials(string json)
//        {
//            UserCredential credential;

//            byte[] byteArray = Encoding.ASCII.GetBytes(json);
//            using (MemoryStream stream = new MemoryStream(byteArray))
//            {
//                // The file token.json stores the user's access and refresh tokens, and is created
//                // automatically when the authorization flow completes for the first time.

//                string path = Path.Combine(Path.GetTempPath(), CredPath);

//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.FromStream(stream).Secrets,
//                    Scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(path, true)).Result;

//                Console.WriteLine("Credential file saved to: " + CredPath);
//            }

//            return credential;
//        }

//        static GoogleCredential ServiceCredentials(string json)
//        {
//            GoogleCredential credential;

//            byte[] byteArray = Encoding.ASCII.GetBytes(json);
//            using (MemoryStream stream = new MemoryStream(byteArray))
//            {
//                var serviceCred = ServiceAccountCredential.FromServiceAccountData(stream);
//                credential = GoogleCredential.FromServiceAccountCredential(serviceCred);
//            }

//            return credential;
//        }
//    }
//}