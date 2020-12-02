using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class CityFinderTest
    {
        [Test]
        public void Find_1234_Main_St_Seattle_WA_City_Seattle()
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
            Assert.AreEqual("Seattle", container.ParsedAddress.City.Value);
            Assert.AreEqual(3, container.ParsedAddress.City.Index);
        }
        [Test]
        public void Find_1234_Main_St_Seattle_City_Seattle()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St Seattle");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("Seattle", container.ParsedAddress.City.Value);
            Assert.AreEqual(3, container.ParsedAddress.City.Index);
        }

        [Test]
        public void Find_1234_Main_St_Apt_15_Seattle_City_Seattle()
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
            Assert.AreEqual("Seattle", container.ParsedAddress.City.Value);
            Assert.AreEqual(5, container.ParsedAddress.City.Index);
        }

        [Test]
        public void Find_1234_Main_St_Apt_15_Federal_Way_City_Federal_Way()
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
            Assert.AreEqual("Federal Way", container.ParsedAddress.City.Value);
            Assert.AreEqual(6, container.ParsedAddress.City.Index);
        }


        [Test]
        public void Find_1234_Main_St_Apt_A_Hypen_15_Federal_Way_City_Federal_Way()
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
            Assert.AreEqual("Federal Way", container.ParsedAddress.City.Value);
            Assert.AreEqual(6, container.ParsedAddress.City.Index);
        }

        [Test]
        public void Find_1234_North_St_StreetName_Main_City_NotSet()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 North St");
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 2;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsTrue(container.ParsedAddress.City.IsNotSet());
        }

        private static CityFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new CityFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}