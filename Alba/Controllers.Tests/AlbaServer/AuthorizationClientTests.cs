using NUnit.Framework;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace AlbaClient.Tests.AlbaServer
{
    [TestFixture]
    public class AuthorizationClientTests
    {
        [Test]
        public void Authorize_WithAuthenticatedTrue_NoException()
        {
            GetClientThatReturns(@"{""authenticated"":true}")
               .Authenticate(BlankCredentials());
        }

        [Test]
        public void Authorize_WithAuthenticatedFalse_ThrowsAlbaAuthorizationException()
        {
            Assert.That (
                () => GetClientThatReturns (@"{""authenticated"":false}")
                    .Authenticate(BlankCredentials ()),
                Throws.TypeOf (typeof (AuthorizationException)));
        }

        [Test]
        public void Authorize_WithIncorrectPassword_ThrowsAlbaAuthorizationException()
        {
            Assert.That (
                () => GetClientThatReturns(@"{""log"":[""Query OK"",""Fetch OK""],""error"":[""Incorrect password.""]}")
                    .Authenticate(BlankCredentials()),
                Throws.TypeOf (typeof (AuthorizationException)));
        }

        [Test]
        public void Authorize_WithInvalidKeys_ThrowsAlbaAuthorizationException()
        {
            Assert.That (
                () => GetClientThatReturns(@"{""error"":[""Invalid keys"",{""an"":""dd"",""us"":""d"",""k2"":""128dc45a5ffe04f7ea0f6ab683df07849b903485""}]}")
                    .Authenticate(BlankCredentials()),
                Throws.TypeOf (typeof (AuthorizationException)));
        }

        [Test]
        public void Authorize_WithTypoMessage_ThrowsAlbaAuthorizationException()
        {
            Assert.That (
                () => GetClientThatReturns(@"{""log"":[""Query OK""],""error"":[""Account name and/or user name are unknown. Typo perhaps?""]}")
                    .Authenticate(BlankCredentials()),
                Throws.TypeOf (typeof (AuthorizationException)));
        }

        private static AuthorizationClient GetClientThatReturns(string message)
        {
            var webClient = new WebClientFake()
            {
                DownloadStringReturns = message,
            };

            var path = new ApplicationBasePath("", "", "");

            return new AuthorizationClient(webClient, path);
        }

        private static Credentials BlankCredentials()
        {
            return new Credentials("", "", "");
        }
    }
}
