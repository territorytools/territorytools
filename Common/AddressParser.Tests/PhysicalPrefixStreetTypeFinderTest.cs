using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class PrefixStreetTypeFinderTest
    {
        [Test]
        public void Find_1234_SR_134_Federal_Way_Sets_SR()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 SR 134 Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("SR", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(1, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_1234_S_R_134_Federal_Way_Sets_S_R()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 S R 134 Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("S R", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(2, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_1234_State_Route_134_Federal_Way_Sets_State_Route()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 State Route 134 Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("State Route", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(2, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_1234_Hwy_555_Federal_Way_Sets_Hwy()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Hwy 555 Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Hwy", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(1, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_1234_Pacific_Hwy_Federal_Way_Ignores_StreetType_IsNull()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Pacific Hwy Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsNull(container.ParsedAddress.StreetType.Value);
        }

        private static PhysicalPrefixStreetTypeFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new PhysicalPrefixStreetTypeFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
