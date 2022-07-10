using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
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
using data = Google.Apis.Sheets.v4.Data;

namespace TerritoryTools.Alba.Controllers
{
    public interface ISpreadSheetService
    {
        void Write(string documentId, string range, IList<IList<object>> values);
        IList<IList<object>> Read(string documentId, string range);
        Spreadsheet CreateSheet(string title, List<Sheet> sheets);
        Spreadsheet GetSpreadsheet(string fromDocumentId);
        void InsertRows(string fromDocumentId, int? sheetId, int v1, int v2);
        void ShareFile(string spreadsheetId, string publisherEmail, GoogleSheets.Role writer);
    }

    public class GoogleSheets : ISpreadSheetService
    {
        // Some APIs, like Storage, accept a credential in their Create()
        // method.
        static readonly string[] Scopes = { 
            SheetsService.Scope.SpreadsheetsReadonly, 
            SheetsService.Scope.Spreadsheets, 
            DriveService.Scope.DriveFile, 
            DriveService.Scope.DriveMetadata,
            DriveService.Scope.Drive
        };

        static string ApplicationName = "Territory Tools";
        const string CredPath = "token2.json";

        readonly SheetsService _service;
        readonly DriveService _driveService;

        public GoogleSheets(string json)
        {
            if (IsJsonForAServiceAccount(json))
            {
                _driveService = new DriveService(BaseClientInit(ServiceCredentials(json)));
                _service = new SheetsService(BaseClientInit(ServiceCredentials(json)));
            }
            else
            {
                _driveService = new DriveService(BaseClientInit(UserCredentials(json)));
                _service = new SheetsService(BaseClientInit(UserCredentials(json)));
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

        public Spreadsheet CreateSheet(string title, List<Sheet> sheets)
        {
            var spreadsheet = new Spreadsheet();
            var properties = new SpreadsheetProperties()
            {
                Title = title,
            };

            spreadsheet.Properties = properties;
            spreadsheet.Sheets = new List<Sheet>();

            foreach (Sheet sheet in sheets)
            {
                //Sheet sheet = new Sheet();
                //sheet.Properties = new SheetProperties()
                //{
                //    Title = name,
                //    GridProperties = new GridProperties()
                //    {
                        
                //    }
                //};

                //if(true)
                //{
                //    sheet.Properties.GridProperties.FrozenRowCount = 1;
                //    sheet.Properties.GridProperties.RowCount = 10;
                //}

                spreadsheet.Sheets.Add(sheet);
            }

            SpreadsheetsResource.CreateRequest request = _service.Spreadsheets.Create(spreadsheet);
            Spreadsheet result = request.Execute();
            Console.WriteLine($"SpreadsheetID: {result.SpreadsheetId}");
            Console.WriteLine($"SpreadsheetUrl: {result.SpreadsheetUrl}");
            foreach(var s in result.Sheets)
            {
                Console.WriteLine($"Sheet: {s.Properties.Title} ID: {s.Properties.SheetId}");
            }

            return result;
        }

        public void InsertRows(string documentId, int? sheetId, int startIndex, int endIndex)
        {
            data.Request insertRowRequest = new data.Request();
            insertRowRequest.InsertDimension = new InsertDimensionRequest()
            {
                InheritFromBefore = false,
                Range = new DimensionRange()
                {
                    Dimension = "ROWS",
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    SheetId = sheetId
                }
            };

            List<data.Request> requests = new List<data.Request>();
            requests.Add(insertRowRequest);

            // TODO: Assign values to desired properties of `requestBody`:
            data.BatchUpdateSpreadsheetRequest requestBody = new data.BatchUpdateSpreadsheetRequest();
            requestBody.Requests = requests;

            SpreadsheetsResource.BatchUpdateRequest req = _service.Spreadsheets.BatchUpdate(requestBody, documentId);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            data.BatchUpdateSpreadsheetResponse response = req.Execute();
        }

        internal object GetSpreadsheet(object fromDocumentId)
        {
            throw new NotImplementedException();
        }

        public Spreadsheet GetSpreadsheet(string documentId)
        {
            SpreadsheetsResource.GetRequest request = _service.Spreadsheets.Get(documentId);
            ////request.Ranges = ranges;
            request.IncludeGridData = false;

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            data.Spreadsheet response = request.Execute();
            return response;
        }

        public void UpdateFrozenRow()
        {
            UpdateSheetPropertiesRequest request = new UpdateSheetPropertiesRequest()
            {
                Properties = new SheetProperties
                {
                    GridProperties = new GridProperties()
                    {
                        FrozenRowCount = 1
                    }
                }
            };
        }

        public enum Role
        {
            Reader,
            Writer,
            FileOrganizer,
            Owner,
        }

        public void ShareFile(string documentId, string email, Role role = Role.Reader)
        {
            try
            {
                Permission userPermission = new Permission()
                {
                    Type = "user",
                    // You must set owner to writer, then set pending owner to get consent
                    Role = CamelCase(role == Role.Owner ? Role.Writer : role),
                    //Role = CamelCase(role),
                    EmailAddress = email,
                    PendingOwner = role == Role.Owner,
                     
                };

                PermissionsResource.CreateRequest request = _driveService.Permissions
                    .Create(userPermission, documentId);

                if (role == Role.Owner)
                {
                    request.TransferOwnership = true;
                    ////request.EmailMessage = "This territory has been assigned to you.";
                    ////request.SendNotificationEmail = true;
                    //request.MoveToNewOwnersRoot = true;
                }

                Permission permResult = request.Execute();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing permissions. {ex.Message}", ex);
            }
        }

        public void AddWriter(string documentId, string email)
        {
            try
            {
                Permission userPermission = new Permission()
                {
                    Type = "user",
                    Role = CamelCase(Role.Writer),
                    EmailAddress = email,
                    PendingOwner = false

                };

                PermissionsResource.CreateRequest request = _driveService.Permissions
                    .Create(userPermission, documentId);

                Permission permResult = request.Execute();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding writer permission. {ex.Message}", ex);
            }
        }

        public static string ColumnName(int number)
        {
            number++; // Input is zero based, output starts at 1

            var builder = new StringBuilder();
            while (number > 0)
            {
                int index = (number - 1) % 26;
                builder.Append((char)(index + 'A'));
                number = (number - 1) / 26;
            }

            string result = builder.ToString();
            char[] reversed = new char[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                reversed[i] = result[result.Length - i - 1];
            }

            return new string(reversed);
        }

        static string CamelCase(Role role)
        {
            return $"{role.ToString().Substring(0, 1).ToLower()}{role.ToString().Substring(1)}";
        }

        private static BaseClientService.Initializer BaseClientInit(ICredential credential)
        {
            return new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            };
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
            GoogleCredential credential = GoogleCredential
                .FromJson(json)
                .CreateScoped(Scopes); 
 
            return credential;
        }
    }
}