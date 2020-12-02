using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class StreetNameFinderTest
    {
        [Test]
        public void Find_1234_Main_St_StreetName_Main()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Main", container.ParsedAddress.StreetName.Value);
            Assert.AreEqual(1, container.ParsedAddress.StreetName.Index);
        }

        [Test]
        public void Find_1234_North_St_StreetName_Main()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("North", container.ParsedAddress.StreetName.Value);
            Assert.AreEqual(1, container.ParsedAddress.StreetName.Index);
        }

        [Test]
        public void Find_1234_M_L_K_Jr_St_StreetName_Main()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 M L K Jr St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 5;
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("M L K Jr", container.ParsedAddress.StreetName.Value);
            Assert.AreEqual(4, container.ParsedAddress.StreetName.Index);
        }


        private static StreetNameFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new StreetNameFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
