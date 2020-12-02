using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class DirectionalSuffixFinderTest 
    {
        [Test]
        public void Find_1234_Main_St_NW_DirectionalSuffix_NW()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St NW");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("NW", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        [Test]
        public void Find_1234_Main_St_N_E_DirectionalSuffix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St N E");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        [Test]
        public void Find_1234_Main_St_N_E_North_Bend_DirectionalSuffix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St N E North Bend");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        [Test]
        public void Find_1234_Main_St_N_North_Bend_DirectionalSuffix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St N North Bend");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        [Test]
        public void Find_1234_Main_St_N_Apt_N_North_Bend_DirectionalSuffix_N()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St N Apt N North Bend");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        [Test]
        public void Find_1234_Main_St_NW_Omaha_NE_DirectionalSuffix_NW()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St NW Omaha NE");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.State.Value = "NE";
            container.ParsedAddress.State.Index = 5; 
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("NW", container.ParsedAddress.DirectionalSuffix.Value);
            Assert.AreEqual(3, container.ParsedAddress.DirectionalSuffix.Index);
        }

        private static DirectionalSuffixFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new DirectionalSuffixFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
