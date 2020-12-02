using NUnit.Framework;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class StreetNameToString : ParserTestBase
    {
        [Test]
        public void ToString_Empty()
        {
            var name = new StreetName();

            Assert.AreEqual(string.Empty, name.ToString());
        }

        [Test]
        public void ToString_Basic()
        {
            var name = new StreetName();
            name.DirectionalPrefix = "N";
            name.Name = "Town Main";
            name.StreetType = "St";
            name.DirectionalSuffix = "SE";

            Assert.AreEqual("N Town Main St SE", name.ToString());
        }

        [Test]
        public void ToString_MissingOnePart()
        {
            var name = new StreetName();
            name.Name = "Town Main";
            name.StreetType = "St";
            name.DirectionalSuffix = "SE";

            Assert.AreEqual("Town Main St SE", name.ToString());
        }

        [Test]
        public void ToString_MissingMiddlePart()
        {
            var name = new StreetName();
            name.DirectionalPrefix = "N";
            name.Name = "Town Main";
            name.DirectionalSuffix = "SE";

            Assert.AreEqual("N Town Main SE", name.ToString());
        }
    }
}
