using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace TerritoryTools.Alba.Controllers.Tests
{
    public class GoogleSheetTests
    {
        [Test]
        public void TestHowSerializationWorks()
        {
            string json = @"
                {
                    ""type"": ""service_account"",
                    ""test_section"": {
                        ""one"": ""test value""
                    }
                }
            ";

            JObject document = JObject.Parse(json);

            var token = document.SelectToken("test_section.one");
            Assert.IsNotNull(token);
            Assert.AreEqual((string)token, "test value");

            var missingToken = document.SelectToken("missing_section.sub_section");
            Assert.IsNull(missingToken);
        }

        [Test]
        public void IsServiceAccountJson()
        {
            string json = @"
                {
                    ""type"": ""service_account"",
                }
            "; 

            Assert.IsTrue(GoogleSheets.IsJsonForAServiceAccount(json));
        }

        [Test]
        public void NotServiceAccountJson()
        {
            string json = @"
                {
                    ""installed"": {
                        ""redirect_uris"": ""test value""
                    }
                }
            ";

            Assert.IsFalse(GoogleSheets.IsJsonForAServiceAccount(json));
        }

        [Test]
        public void CreateTest()
        {
            string json = File.ReadAllText("./client.secrets.json");
            Assert.IsNotNull(json);

            GoogleSheets googleSheets = new GoogleSheets(json);
            Assert.IsNotNull(googleSheets);

            Spreadsheet sheet = googleSheets.CreateSheet($"Test Sheet {DateTime.Now.ToString("yyyy-MM-dd_hhmmss")}");
            //Assert.IsNotNull(sheet.SpreadsheetUrl);

            IList<IList<object>> values = new List<IList<object>>()
            {
                new List<object>() { "Hi", "there", "world "},
                new List<object>() { null, null, null},
                new List<object>() { "Hello", null, "marc"},
            };

            googleSheets.Write(sheet.SpreadsheetId, "Requests!A1:C3", values);
            // TODO: This could be tested by downloading the CSV for the google sheet...
        }
    }
}
