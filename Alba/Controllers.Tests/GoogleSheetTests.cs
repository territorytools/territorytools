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
        public void ColumnNameTest()
        {
            Assert.AreEqual("A", GoogleSheets.ColumnName(0));
            Assert.AreEqual("B", GoogleSheets.ColumnName(1));
            Assert.AreEqual("Z", GoogleSheets.ColumnName(25));
            Assert.AreEqual("AA", GoogleSheets.ColumnName(26));
            Assert.AreEqual("AB", GoogleSheets.ColumnName(27));
            Assert.AreEqual("ZZ", GoogleSheets.ColumnName(701));
            Assert.AreEqual("AAA", GoogleSheets.ColumnName(702));
            Assert.AreEqual("AAB", GoogleSheets.ColumnName(703));
        }
    }
}
