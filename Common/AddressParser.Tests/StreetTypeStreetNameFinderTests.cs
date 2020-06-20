using NUnit.Framework;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;

namespace TerritoryEntities.Tests.AddressParsers
{
    [TestFixture]
    public class StreetTypeStreetNameFinderTests
    {
        [Test]
        public void Find_Broadway_E_Is_A_StreetNameWithNoStreetType()
        {
            // Arrange
            var container = new AddressParseContainer(@"523 Broadway E Apt 664, Seattle WA 98102-5381");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Broadway", container.ParsedAddress.StreetName.Value);
        }

        private static StreetTypeStreetNameFinder GetFinder(AddressParseContainer container)
        {
            var streetTypes = StreetType.From(
                new string[]
                {
                    "ALLEY",
                    "AVENUE",
                    "CIRCLE",
                    "WAY"
                });

            var splitter = new AddressSplitter(container);
            var finder = new StreetTypeStreetNameFinder(container, streetTypes);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
