using System;
using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class UnitTypeFinderTest
    {

        [Test]
        public void Find_1234_Main_St_Apt_15_Sets_Apt()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 Main St Apt 15");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Apt", container.ParsedAddress.UnitType.Value);
            Assert.AreEqual(3, container.ParsedAddress.UnitType.Index);
        }

        [Test]
        public void Find_1111_Main_St_Apt_22_Seattle_WA_98144_3333_Sets_Apt()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St Apt 22 Seattle WA 98144-3333");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Apt", container.ParsedAddress.UnitType.Value);
            Assert.AreEqual(3, container.ParsedAddress.UnitType.Index);
        }

        [Test]
        public void Find_1111_Main_St_Pound22_Seattle_WA_98144_3333_Sets_Pound()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St #22 Seattle WA 98144-3333");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("#", container.ParsedAddress.UnitType.Value);
            Assert.AreEqual(3, container.ParsedAddress.UnitType.Index);
        }

        [Test]
        public void Find_1111_Main_St_Pound_22_Seattle_WA_98144_3333_Sets_Pound()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St # 22 Seattle WA 98144-3333");
            container.ParsedAddress.StreetType.Index = 2; 
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("#", container.ParsedAddress.UnitType.Value);
            Assert.AreEqual(3, container.ParsedAddress.UnitType.Index);
        }

        private static UnitTypeFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new UnitTypeFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
