using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class UnitNumberFinderTest
    {
        [Test]
        public void Find_1234_Main_St_Seattle_WA_UnitNumber_NotSet()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St, Seattle, WA");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.State.Value = "WA";
            container.ParsedAddress.State.Index = 4;

            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsTrue(container.ParsedAddress.UnitNumber.IsNotSet());
        }

        [Test]
        public void Find_1234_Main_St_Apt_15_Seattle_UnitNumber_15()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St Apt 15 Seattle");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.UnitType.Value = "Apt";
            container.ParsedAddress.UnitType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("15", container.ParsedAddress.UnitNumber.Value);
            Assert.AreEqual(4, container.ParsedAddress.UnitNumber.Index);
        }

        [Test]
        public void Find_1234_Main_St_Apt_15_Federal_Way_UnitNumber_15()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St Apt 15 Federal Way");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.UnitType.Value = "Apt";
            container.ParsedAddress.UnitType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("15", container.ParsedAddress.UnitNumber.Value);
            Assert.AreEqual(4, container.ParsedAddress.UnitNumber.Index);
        }


        [Test]
        public void Find_1234_Main_St_Apt_A_Hypen_15_Federal_Way_UnitNumber_A_15()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St Apt A-15 Federal Way");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.UnitType.Value = "Apt";
            container.ParsedAddress.UnitType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("A-15", container.ParsedAddress.UnitNumber.Value);
            Assert.AreEqual(4, container.ParsedAddress.UnitNumber.Index);
        }

        private static UnitNumberFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new UnitNumberFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}