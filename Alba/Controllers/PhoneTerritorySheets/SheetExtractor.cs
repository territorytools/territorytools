using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public interface ISheetExtractor
    {
        string Extract(SheetExtractionRequest request);
        string AddSheetWriter(AddSheetWriterRequest request);
    }

    public class SheetExtractor : ISheetExtractor
    {
        GoogleSheets _googleSheets;

        public string Extract(SheetExtractionRequest request)
        {
            _googleSheets = new GoogleSheets(request.SecurityToken);

            List<AssignmentRow> assignmentRows = LoadAssignments(request.FromDocumentId, "Assignments");

            string requestedTerritoryNumber = request.TerritoryNumber;
            if (string.Equals(request.TerritoryNumber, "NEW", StringComparison.OrdinalIgnoreCase)
                || string.Equals(request.TerritoryNumber, "NEXT", StringComparison.OrdinalIgnoreCase))
            {
                requestedTerritoryNumber = assignmentRows
                    .Where(row => string.Equals(row.Transaction, "Available", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(row => row.TerritoryNumber)
                    .Take(1)
                    .SingleOrDefault()
                    ?.TerritoryNumber;
            }

            if(string.IsNullOrWhiteSpace(requestedTerritoryNumber))
            {
                throw new Exception("Cannot find available territory number");
            }

            List <PhoneRow> allPhoneRows = LoadPhoneRows(request.FromDocumentId, request.FromSheetName);
            List<PhoneRow> phoneRows = allPhoneRows
                .Where(r => r.TerritoryNumber == requestedTerritoryNumber)
                .ToList();

            foreach (PhoneRow phoneRow in phoneRows)
            {
                if (string.IsNullOrWhiteSpace(phoneRow.Publisher))
                {
                    phoneRow.Publisher = request.PublisherName;
                }
            }

            int phoneRowStartIndex = allPhoneRows.IndexOf(phoneRows[0]);
            int phoneRowEndIndex = phoneRowStartIndex + phoneRows.Count;

            Spreadsheet sheet = _googleSheets.CreateSheet(
                title: $"Territory {requestedTerritoryNumber}",
                sheets: new List<Sheet> { PhoneNumberSheet(phoneRows), ResultsSheet() });

            // TODO: Instead of creating Results sheet this way, copy this from the master sheet
            _googleSheets.Write(sheet.SpreadsheetId, "Results!A1:A10", ResultsList());

            PrepareForSheet(phoneRows);

            // TODO: Compare data?
            // TODO: Sometimes delete the source data (from the public sheet)
            // TODO: Update 'Checkout' status too (for now)
            _googleSheets.Write(
                documentId: request.FromDocumentId,
                range: $"{request.FromSheetName}!A{phoneRowStartIndex + 1}:J{phoneRowEndIndex + 1}",
                values: CheckedOutRows(phoneRows, request.PublisherName));

            var spreadsheet = _googleSheets.GetSpreadsheet(request.FromDocumentId);
            var log = spreadsheet.Sheets.FirstOrDefault(sh => "AssignmentLog".Equals(sh.Properties?.Title));
            if (log == null)
            {
                throw new Exception("Cannot find AssignmentLog sheet");
            }

            _googleSheets.InsertRows(request.FromDocumentId, log.Properties.SheetId, 1, 2);

            var logRowContents = new List<object> {
                        DateTime.Today.ToShortDateString(),
                        requestedTerritoryNumber,
                        request.PublisherName,
                        "Checked Out",
                        null,
                        sheet.SpreadsheetUrl};

            IList<IList<object>> logRow = new List<IList<object>>
                {
                   logRowContents
                };

            string logEndColumnName = GoogleSheets.ColumnName(logRowContents.Count);
            _googleSheets.Write(request.FromDocumentId, $"AssignmentLog!A2:{logEndColumnName}2", logRow);

            var assignment = assignmentRows.FirstOrDefault(row => row.TerritoryNumber == requestedTerritoryNumber);
            int assignmentRowNumber = assignmentRows.IndexOf(assignment) + 1;
            var assignmentRowContents = new List<object> {
                        requestedTerritoryNumber,
                        DateTime.Today.ToShortDateString(),
                        "Checked Out",
                        request.PublisherName,
                        null,
                        sheet.SpreadsheetUrl};

            IList<IList<object>> assignmentRow = new List<IList<object>>
                {
                   assignmentRowContents
                };

            string assignmentEndColumnName = GoogleSheets.ColumnName(assignmentRowContents.Count);
            _googleSheets.Write(request.FromDocumentId, $"Assignments!A{assignmentRowNumber}:{logEndColumnName}{assignmentRowNumber}", assignmentRow);

            _googleSheets.ShareFile(sheet.SpreadsheetId, request.PublisherEmail, GoogleSheets.Role.Writer);
            _googleSheets.ShareFile(sheet.SpreadsheetId, request.OwnerEmail, GoogleSheets.Role.Writer);

            return sheet.SpreadsheetUrl;
        }

        public string Assign(AssignSheetRequest request)
        {
            _googleSheets = new GoogleSheets(request.SecurityToken);

            _googleSheets.ShareFile(request.DocumentId, request.PublisherEmail, GoogleSheets.Role.Writer);
            _googleSheets.ShareFile(request.DocumentId, request.OwnerEmail, GoogleSheets.Role.Owner);

            return $"https://docs.google.com/spreadsheets/d/{request.DocumentId}";
        }

        public string AddSheetWriter(AddSheetWriterRequest request)
        {
            _googleSheets = new GoogleSheets(request.SecurityToken);

            _googleSheets.ShareFile(request.DocumentId, request.UserEmail, GoogleSheets.Role.Writer);

            return $"https://docs.google.com/spreadsheets/d/{request.DocumentId}";
        }


        public void LogMessage(SmsMessage message)
        {
            _googleSheets = new GoogleSheets(message.SecurityToken);

            Spreadsheet spreadsheet = _googleSheets.GetSpreadsheet(message.LogDocumentId);
            Sheet log = spreadsheet.Sheets.FirstOrDefault(sh => 
                !string.IsNullOrWhiteSpace(message.LogSheetName) 
                && message.LogSheetName.Equals(sh.Properties?.Title));

            if (log == null)
            {
                throw new Exception("Cannot find Messages sheet");
            }

            _googleSheets.InsertRows(message.LogDocumentId, log.Properties.SheetId, 1, 2);

            var messageRowContents = new List<object> {
                        message.Timestamp,
                        message.Id,
                        message.To,
                        message.From,
                        message.Message};

            IList<IList<object>> messageRow = new List<IList<object>>
                {
                   messageRowContents
                };

            string logEndColumnName = GoogleSheets.ColumnName(messageRowContents.Count);
            _googleSheets.Write(message.LogDocumentId, $"Messages!A2:{logEndColumnName}2", messageRow);
        }

        private static Sheet ResultsSheet()
        {
            return new Sheet
            {
                Properties = new SheetProperties
                {
                    Title = "Results",
                    GridProperties = new GridProperties
                    {
                        RowCount = 10,
                        ColumnCount = 1,
                    }
                }
            };
        }

        private static Sheet PhoneNumberSheet(List<PhoneRow> phoneRows)
        {
            var columnHeadings = new List<string>() {
                "Order",
                "Publisher",
                "Category",
                "Phone",
                "Phone 1 Results",
                "Phone 2 Results",
                "Notes",
                "Date",
                "Territory",
                "Added"
            };

            Sheet phoneNumberSheet = new Sheet
            {
                Properties = new SheetProperties
                {
                    Title = "Phone Numbers",
                    GridProperties = new GridProperties
                    {
                        RowCount = phoneRows.Count + 1,
                        ColumnCount = columnHeadings.Count,
                        FrozenRowCount = 1,
                    }
                }
            };

            phoneNumberSheet.Data = new List<GridData>
            {
                new GridData
                {
                    RowData = new List<RowData> {
                        new RowData {
                             Values = new List<CellData>()
                        }
                    }
                }
            };

            //Make top row bold, 10 columns
            foreach (string heading in columnHeadings)
            {
                phoneNumberSheet.Data[0].RowData[0].Values.Add(
                    BoldStringCellData(heading));
            }

            // Add data with formatting and validation
            for (int r = 0; r < phoneRows.Count; r++)
            {
                PhoneRow phoneRow = phoneRows[r];
                phoneNumberSheet.Data[0].RowData.Add(
                    new RowData
                    {
                        Values = new List<CellData>()
                        {
                            StringCellData(phoneRow.Order),
                            StringCellData(phoneRow.Publisher),
                            StringCellData(phoneRow.Category),
                            StringCellData(phoneRow.PhoneNumber),
                            ResultsDropDownCellData(phoneRow.PhoneResults1),
                            ResultsDropDownCellData(phoneRow.PhoneResults2),
                            StringCellData(phoneRow.Notes),
                            StringCellData(phoneRow.Date),
                            StringCellData(phoneRow.TerritoryNumber),
                            StringCellData(phoneRow.Added)
                        }
                    }
                );
            }

            return phoneNumberSheet;
        }

        private static IList<IList<object>> CheckedOutRows(List<PhoneRow> phoneRows, string publisherName)
        {
            IList<IList<object>> checkedOutRows = new List<IList<object>>();
            foreach (PhoneRow phoneRow in phoneRows)
            {
                checkedOutRows.Add(new List<object>()
                {
                    null, //phoneRow.Order,
                    string.IsNullOrWhiteSpace(phoneRow.Publisher) ? publisherName : null,
                    null, //phoneRow.Category,
                    null, //phoneRow.PhoneNumber,
                    null, //phoneRow.PhoneResults1,
                    null, //phoneRow.PhoneResults2,
                    null, //phoneRow.Notes,
                    null, //phoneRow.Date,
                    null, //phoneRow.TerritoryNumber,
                    null, //phoneRow.Added
                });
            };
            return checkedOutRows;
        }

        private static IList<IList<object>> ResultsList()
        {
            return new List<IList<object>>()
            {
                new List<object>() { "Skipped" },
                new List<object>() { "Disconnected" },
                new List<object>() { "No Answer" },
                new List<object>() { "Left Message" },
                new List<object>() { "Left Message (Chinese)" },
                new List<object>() { "Left Message (Not Chinese)" },
                new List<object>() { "Busy Tone" },
                new List<object>() { "Not Chinese" },
                new List<object>() { "Chinese" },
                new List<object>() { "See Notes" },
            };
        }

        static IList<IList<object>> PrepareForSheet(List<PhoneRow> phoneRows)
        {
            IList<IList<object>> newNumbers = new List<IList<object>>()
            {
                new List<object>() {
                    "Order",
                    "Publisher",
                    "Category",
                    "Phone",
                    "Phone 1 Results",
                    "Phone 2 Results",
                    "Notes",
                    "Date",
                    "Territory",
                    "Added"
                },
            };

            foreach (PhoneRow phoneRow in phoneRows)
            {
                newNumbers.Add(
                    new List<object>()
                    {
                        phoneRow.Order,
                        phoneRow.Publisher,
                        phoneRow.Category,
                        phoneRow.PhoneNumber,
                        phoneRow.PhoneResults1,
                        phoneRow.PhoneResults2,
                        phoneRow.Notes,
                        phoneRow.Date,
                        phoneRow.TerritoryNumber,
                        phoneRow.Added
                    });
            }

            return newNumbers;
        }

        List<PhoneRow> LoadPhoneRows(string documentId, string sheetName)
        {
            IList<IList<object>> masterPhoneList = _googleSheets.Read(
               documentId: documentId,
               range: sheetName);

            List<PhoneRow> allPhoneRows = new List<PhoneRow>();
            foreach (IList<object> row in masterPhoneList)
            {
                allPhoneRows.Add(new PhoneRow
                {
                    Order = row.Count > 0 ? row[0] as string : null,
                    Publisher = row.Count > 1 ? row[1] as string : null,
                    Category = row.Count > 2 ? row[2] as string : null,
                    PhoneNumber = row.Count > 3 ? row[3] as string : null,
                    PhoneResults1 = row.Count > 4 ? row[4] as string : null,
                    PhoneResults2 = row.Count > 5 ? row[5] as string : null,
                    Notes = row.Count > 6 ? row[6] as string : null,
                    Date = row.Count > 7 ? row[7] as string : null,
                    TerritoryNumber = row.Count > 8 ? row[8] as string : null,
                    Added = row.Count > 9 ? row[9] as string : null,
                });
            }

            return allPhoneRows;
        }

        List<AssignmentRow> LoadAssignments(string documentId, string range)
        {
            IList<IList<object>> assignmentList = _googleSheets.Read(
                documentId: documentId,
                range: range);

            List<AssignmentRow> assignmentRows = new List<AssignmentRow>();
            foreach (IList<object> row in assignmentList)
            {
                assignmentRows.Add(new AssignmentRow
                {
                    TerritoryNumber = row.Count > 0 ? row[0] as string : null,
                    Date = row.Count > 1 ? row[1] as string : null,
                    Transaction = row.Count > 2 ? row[2] as string : null,
                    Publisher = row.Count > 3 ? row[3] as string : null,
                    Notes = row.Count > 4 ? row[4] as string : null,
                    DocumentLink = row.Count > 5 ? row[5] as string : null,
                });
            }
            
            return assignmentRows;
        }

        private static CellData ResultsDropDownCellData(string value)
        {
            return new CellData
            {
                UserEnteredValue = new ExtendedValue
                {
                    StringValue = value,
                },
                DataValidation = new DataValidationRule
                {
                    Condition = new BooleanCondition
                    {
                        Type = "ONE_OF_RANGE",
                        Values = new List<ConditionValue>()
                        {
                            new ConditionValue
                            {
                                UserEnteredValue = "=Results!A:A"
                            }
                        },
                    },
                    Strict = false,
                    ShowCustomUi = true,
                }
            };
        }

        private static CellData StringCellData(string value)
        {
            return new CellData
            {
                UserEnteredValue = new ExtendedValue
                {
                    StringValue = value,
                }
            };
        }

        private static CellData BoldStringCellData(string value)
        {
            return new CellData
            {
                UserEnteredValue = new ExtendedValue
                {
                    StringValue = value,
                },
                UserEnteredFormat = new CellFormat
                {
                    TextFormat = new TextFormat
                    {
                        Bold = true,
                    }
                },
            };
        }
    }
}
