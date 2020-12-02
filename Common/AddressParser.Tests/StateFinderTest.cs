using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class StateFinderTest
    {

        [Test]
        public void Find_1111_Main_St_98144_Ignores()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St 98144");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsNull(container.ParsedAddress.State.Value);
        }

        [Test]
        public void Find_1111_Main_St_Seattle_WA_98144_Sets_WA()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St Seattle WA 98144");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("WA", container.ParsedAddress.State.Value);
            Assert.AreEqual(4, container.ParsedAddress.State.Index);
        }

        [Test]
        public void Find_1111_Main_St_Federal_Way_WA_98144_Sets_WA()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St Federal Way WA 98144");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("WA", container.ParsedAddress.State.Value);
            Assert.AreEqual(5, container.ParsedAddress.State.Index);
        }

        [Test]
        public void Find_1111_Main_St_Apt_22_Seattle_WA_98144_3333_Sets_WA()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St Apt 22 Seattle WA 98144-3333");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.UnitType.Index = 3;
            container.ParsedAddress.UnitType.Value = "Apt";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("WA", container.ParsedAddress.State.Value);
            Assert.AreEqual(6, container.ParsedAddress.State.Index);
        }

        private static StateFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new StateFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
