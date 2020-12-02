using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class AddressNumberFinderTest
    {
        [Test]
        public void Find_1234_Main_St_Sets_1234()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("1234", container.ParsedAddress.Number.Value);
            Assert.AreEqual(0, container.ParsedAddress.Number.Index);
        }

        [Test]
        public void Find_PO_Box_1234_Sets_1234()
        {
            // Arrange
            var container = new AddressParseContainer(@"PO Box 1234");
            container.Address.IsNotPhysical = true;
            container.ParsedAddress.StreetType.Value = "PO Box";
            container.ParsedAddress.StreetType.Index = 1;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("1234", container.ParsedAddress.Number.Value);
            Assert.AreEqual(2, container.ParsedAddress.Number.Index);
        }

        private static AddressNumberFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new AddressNumberFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
