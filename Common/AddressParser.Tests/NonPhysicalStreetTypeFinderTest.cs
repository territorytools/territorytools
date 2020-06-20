using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class NonStreetTypeFinderTest
    {


        [Test]
        public void Find_PO_Box_1234_Federal_Way_Sets_PO_Box()
        {
            // Arrange
            var container = new AddressParseContainer(@"PO Box 1234 Federal Way 98123");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("PO Box", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(1, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_POB_1234_Federal_Way_Sets_POB()
        {
            // Arrange
            var container = new AddressParseContainer(@"POB 1234 Federal Way 98123");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("POB", container.ParsedAddress.StreetType.Value);
            Assert.AreEqual(0, container.ParsedAddress.StreetType.Index);
        }

        [Test]
        public void Find_PO_Box_NoNumberHere_Federal_Way_Ignores_IsNull()
        {
            // Arrange
            var container = new AddressParseContainer(@"PO Box NoNumberHere Federal Way 98123");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsNull(container.ParsedAddress.StreetType.Value);
        }


        [Test]
        public void Find_PO_Box_1234_Federal_Way_IsNonPhysical_True()
        {
            // Arrange
            var container = new AddressParseContainer(@"PO Box 1234 Federal Way");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsTrue(container.Address.IsNotPhysical);
        }

        [Test]
        public void Find_1234_Main_St_Seattle_IsNonPhysical_False()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St Seattle");
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsFalse(container.Address.IsNotPhysical);
        }

        private static NonPhysicalStreetTypeFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new NonPhysicalStreetTypeFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
