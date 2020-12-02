using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class AddressNumberFractionFinderTest
    {
        [Test]
        public void Find_1234_onehalf_Main_St_NumberFraction_onehalf()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 1/2 Main St");
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("1/2", container.ParsedAddress.NumberFraction.Value);
            Assert.AreEqual(1, container.ParsedAddress.NumberFraction.Index);
        }

        // Conflict with ParseAddress_8428_M_L_King_Jr_Way_S
        // TODO: Resurrect this one day
        //[Test]
        public void Find_1234_B_Main_St_NumberFraction_onehalf()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 B Main St");
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("B", container.ParsedAddress.NumberFraction.Value);
            Assert.AreEqual(1, container.ParsedAddress.NumberFraction.Index);
        }

        // Conflict with ParseAddress_8428_M_L_King_Jr_Way_S
        // TODO: Resurrect this one day
        //[Test]
        public void Find_1234_A_B_St_NumberFraction_onehalf()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 A B St");
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("B", container.ParsedAddress.NumberFraction.Value);
            Assert.AreEqual(1, container.ParsedAddress.NumberFraction.Index);
        }

        [Test]
        public void Find_1234_N_B_St_NumberFraction_Null()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 N B St");
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 3;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.IsNull(container.ParsedAddress.NumberFraction.Value);
        }

        // Conflict with ParseAddress_8428_M_L_King_Jr_Way_S
        // TODO: Resurrect this one day
        // [Test]
        public void Find_1234_N_S_B_St_NumberFraction_onehalf()
        {
            // Arrange
            var container = new AddressParseContainer(@"1234 N S B St");
            container.ParsedAddress.Number.Value = "1234";
            container.ParsedAddress.Number.Index = 0;
            container.ParsedAddress.StreetType.Value = "St";
            container.ParsedAddress.StreetType.Index = 4;
            var finder = GetFinder(container);

            // Act
            finder.Find();

            // Assert
            Assert.AreEqual("N", container.ParsedAddress.NumberFraction.Value);
            Assert.AreEqual(1, container.ParsedAddress.NumberFraction.Index);
        }

        private static AddressNumberFractionFinder GetFinder(AddressParseContainer container)
        {
            var splitter = new AddressSplitter(container);
            var finder = new AddressNumberFractionFinder(container);
            splitter.SplitAndClean();

            return finder;
        }
    }
}
