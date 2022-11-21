using NUnit.Framework;
using System.Text.Json;

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

            JsonElement document = JsonDocument.Parse(json).RootElement;

            string token = document
                .GetProperty("test_section")
                .GetProperty("one")
                .GetString();

            Assert.IsNotNull(token);
            Assert.AreEqual(token, "test value");

            string missingToken = document
                .GetProperty("missing_section")
                .GetProperty("sub_section")
                .GetString();

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
