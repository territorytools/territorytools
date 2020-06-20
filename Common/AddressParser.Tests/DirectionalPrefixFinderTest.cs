using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class DirectionalPrefixFinderTest 
    {
        [Test]
        public void Find_1234_NE_Main_St_DirectionalPrefix_NE()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 NE Main St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("NE", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(1, container.ParsedAddress.DirectionalPrefix.Index);
        }

        [Test]
        public void Find_1234_N_NE_Main_St_DirectionalPrefix_NE()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 N NE Main St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 4;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("NE", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(2, container.ParsedAddress.DirectionalPrefix.Index);
        }

        [Test]
        public void Find_1234_E_N_S_St_DirectionalPrefix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 E N S St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 4;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(2, container.ParsedAddress.DirectionalPrefix.Index);
        }

        [Test]
        public void Find_1234_E_E_St_E_DirectionalPrefix_Index1_E_EitherOr()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 E E St E");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("E", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(1, container.ParsedAddress.DirectionalPrefix.Index);
        }

        [Test]
        public void Find_1234_S_North_St_DirectionalPrefix_S()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 S North St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("S", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(1, container.ParsedAddress.DirectionalPrefix.Index);
        }

        [Test]
        public void Find_1234_N_Main_St_E_DirectionalPrefix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 N Main St E");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalPrefix.Value);
            Assert.AreEqual(1, container.ParsedAddress.DirectionalPrefix.Index);
        }

        private static DirectionalPrefixFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new DirectionalPrefixFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
