using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class PostalCodeFinderTest
    {

        [Test]
        public void Find_1111_Main_St_98144_Sets_98144()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St 98144");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("98144", container.ParsedAddress.PostalCode.Value);
            Assert.AreEqual(3, container.ParsedAddress.PostalCode.Index);
        }

        [Test]
        public void Find_1111_Main_St_Seattle_WA_98144_Sets_98144()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St Seattle WA 98144");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("98144", container.ParsedAddress.PostalCode.Value);
            Assert.AreEqual(5, container.ParsedAddress.PostalCode.Index);
        }

        [Test]
        public void Find_1111_Main_St_Pound_22_Seattle_WA_98144_3333_Sets_98144()
        {
            // Arrange
            var container = new AddressParseContainer(@"1111 Main St # 22 Seattle WA 98144-3333");
            container.ParsedAddress.StreetType.Index = 2;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.UnitType.Index = 3;
            container.ParsedAddress.UnitType.Value = "#";
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("98144", container.ParsedAddress.PostalCode.Value);
            Assert.AreEqual(7, container.ParsedAddress.PostalCode.Index);
        }

        private static PostalCodeFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new PostalCodeFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
