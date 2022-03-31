using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Alba.Controllers.PhoneTerritorySheets
{
    public class SheetExtractor
    {
        public string Extract(SheetExtractionRequest request)
        {
            var googleSheets = new GoogleSheets(request.SecurityToken);

            IList<IList<object>> masterPhoneList = googleSheets.Read(
                documentId: request.FromDocumentId,
                range: request.FromSheetName);

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

            List<PhoneRow> phoneRows = allPhoneRows
                .Where(r => r.TerritoryNumber == request.TerritoryNumber)
                .ToList();

            foreach(PhoneRow phoneRow in phoneRows)
            {
                if(string.IsNullOrWhiteSpace(phoneRow.Publisher))
                {
                    phoneRow.Publisher = request.PublisherName;
                }
            }

            int phoneRowStartIndex = allPhoneRows.IndexOf(phoneRows[0]);
            int phoneRowEndIndex = phoneRowStartIndex + phoneRows.Count;

            Sheet resultsSheet = new Sheet
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

            Sheet phoneNumberSheet = new Sheet
            {
                Properties = new SheetProperties
                {
                    Title = "Phone Numbers",
                    GridProperties = new GridProperties
                    {
                        RowCount = phoneRows.Count + 1,
                        ColumnCount = 7, //10,
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

            var columnHeadings = new List<string>() {
                //"Order",
                "Publisher",
                "Category",
                "Phone",
                "Phone 1 Results",
                "Phone 2 Results",
                "Notes",
                "Date",
                //"Territory",
                //"Added"
            };

            //Make top row bold, 10 columns
            foreach(string heading in columnHeadings)
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
                            //StringCellData(phoneRow.Order),
                            StringCellData(phoneRow.Publisher),
                            StringCellData(phoneRow.Category),
                            StringCellData(phoneRow.PhoneNumber),
                            ResultsDropDownCellData(phoneRow.PhoneResults1),
                            ResultsDropDownCellData(phoneRow.PhoneResults2),
                            StringCellData(phoneRow.Notes),
                            StringCellData(phoneRow.Date),
                            //StringCellData(phoneRow.TerritoryNumber),
                            //StringCellData(phoneRow.Added)
                        }
                    }
                );
            }

            Spreadsheet sheet = googleSheets.CreateSheet(
                title: $"Territory {request.TerritoryNumber}",
                sheets: new List<Sheet> { phoneNumberSheet, resultsSheet });

            // TODO: Instead of creating Results sheet this way, copy this from the master sheet
            IList<IList<object>> values = new List<IList<object>>()
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

            googleSheets.Write(sheet.SpreadsheetId, "Results!A1:A10", values);

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

            // TODO: Update 'Checkout' status too (for now)
            IList<IList<object>> checkedOutRows = new List<IList<object>>();
            foreach (PhoneRow phoneRow in phoneRows)
            {
                checkedOutRows.Add(new List<object>()
                {
                    null, //phoneRow.Order,
                    string.IsNullOrWhiteSpace(phoneRow.Publisher) ? request.PublisherName : null,
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

            googleSheets.Write(request.FromDocumentId, $"{request.FromSheetName}!A{phoneRowStartIndex + 1}:J{phoneRowEndIndex + 1}", checkedOutRows);

            var spreadsheet = googleSheets.GetSpreadsheet(request.FromDocumentId);
            var log = spreadsheet.Sheets.FirstOrDefault(sh => "CheckOutLog".Equals(sh.Properties?.Title));
            googleSheets.InsertRows(request.FromDocumentId, log.Properties.SheetId, 1, 2);
            IList<IList<object>> logRow = new List<IList<object>>
                {
                    new List<object> { DateTime.Today.ToShortDateString(), request.TerritoryNumber, request.PublisherName, "Checked Out", "No notes"}
                };

            googleSheets.Write(request.FromDocumentId, $"CheckOutLog!A2:E2", logRow);

            googleSheets.ShareFile(sheet.SpreadsheetId, request.PublisherEmail, GoogleSheets.Role.Writer);
            googleSheets.ShareFile(sheet.SpreadsheetId, request.OwnerEmail, GoogleSheets.Role.Owner);

            return sheet.SpreadsheetUrl;
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
