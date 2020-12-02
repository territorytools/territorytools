using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;
using TerritoryTools.Entities;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class NormalStreetTypeFinderTest
    {
        [Test]
        public void Find_1234_Main_Street_Set_Street()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main Street");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Street", container.ParsedAddress.StreetType.Value);
        }

        [Test]
        public void Find_1234_Main_St_Set_St()
        {
            // Arrange
            var container = new AddressParseContainer( @"1234 Main St");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("St", container.ParsedAddress.StreetType.Value);
        }

        [Test]
        public void Find_Main_St_Set_Index_2()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual(2, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_Street_Rd_Set_Rd()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main Street Rd");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Rd", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(3, container.ParsedAddress.StreetType.Index);
        }


        [Test]
        public void Find_Mall_Square_Set_Square()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Bellevue Mall Square");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Square", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(3, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_Square_Mall_Set_Mall()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Bellevue Square Mall");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Mall", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(3, container.ParsedAddress.StreetType.Index);
        }
        [Test]
        public void Find_Mall_Sq_Set_Sq()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Bellevue Mall Sq");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Sq", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(3, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_1234_Main_Street_Rd_Federal_Way_Set_Rd()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main Street Rd Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.Ignore();
            Assert.AreEqual("Rd", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(3, container.ParsedAddress.StreetType.Index);
        }


        private static NormalStreetTypeFinder GetFinder(AddressParseContainer container)
        {
            var streetTypes = StreetType.From(
                new string[]
                {
                    "ALLEY",
                    "AVENUE",
                    "BOULEVARD",
                    "CIRCLE",
                    "CONNECTOR",
                    "COURT",
                    "DRIVE",
                    "HIGHWAY",
                    "KEY",
                    "LANE",
                    "LOOP",
                    "MALL",
                    "PARKWAY",
                    "PLACE",
                    "ROAD",
                    "STREET",
                    "SQUARE",
                    "TERRACE",
                    "WAY"
                });

            var splitter = new AddressSplitter(container);
            var finder = new NormalStreetTypeFinder(container, streetTypes);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
