﻿using Google.Apis.Auth.OAuth2;
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
    public interface ISheets
    {
        void Write(string documentId, string range, IList<IList<object>> values);
        IList<IList<object>> Read(string documentId, string range);
        Spreadsheet CreateSheet(string title, List<Sheet> sheets);
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