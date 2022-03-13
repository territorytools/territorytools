using Newtonsoft.Json.Linq;
using NUnit.Framework;

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
    }
}
