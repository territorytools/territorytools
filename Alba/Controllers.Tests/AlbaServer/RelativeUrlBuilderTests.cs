using NUnit.Framework;
using AlbaClient.Models;
using AlbaClient.AlbaServer;

namespace AlbaClient.Tests.AlbaServer
{
    [TestFixture]
    public class RelativeUrlBuilderTests
    {
        [Test]
        public void Authorize_WithAllCredentials_ReturnsCorrectHash()
        {
            var credentials = new Credentials("account1", "user1", "P@ssw0rd");
            credentials.K1MagicString = "test-string-to-hash-with";

            var url = RelativeUrlBuilder.AuthenticationUrlFrom(credentials);

            Assert.AreEqual("ce24996002987b88a0cab15bf7538b4ee649ce19", ExtractHashFrom(url));
        }

        private static string ExtractHashFrom(string url)
        {
            return url.Substring(url.IndexOf("k2=") + 3);
        }

        [Test]
        public void RequestToAddNew_WithTestTerritory_ReturnsCorrectlyEncodedUrl()
        {
            string expected = @"/ts?mod=territories&cmd=add&kind=0&number=TestNumber-123&notes=Test+Notes&description=Test+Description&border=1.11+-2.22%2c-3.33+-4.44";

            var territory = new Territory("TestId")
            {
                Number = "TestNumber-123",
                Description = "Test Description",
                Notes = "Test Notes",
            };

            territory.Border.Vertices.Add(new Vertex(1.11, -2.22));
            territory.Border.Vertices.Add(new Vertex(-3.33, -4.44));

            string url = RelativeUrlBuilder.RequestToAddNew(territory);

            Assert.AreEqual(expected, url);
        }
    }
}
