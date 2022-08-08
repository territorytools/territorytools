﻿using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class AddressSplitterTest
    {
        [Test]
        public void SplitAtPoundSigns_1111_Pound2222_Returns_EachPart()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("#2222");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtPoundSigns();

            // Assert
            Assert.AreEqual("1111", container.AddressParts[0]);
            Assert.AreEqual("#", container.AddressParts[1]);
            Assert.AreEqual("2222", container.AddressParts[2]);
        }


        [Test]
        public void SplitAtPoundSigns_1111_Pound2222_Returns_Parts_Count_3()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("#2222");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtPoundSigns();

            // Assert
            Assert.AreEqual(3, container.AddressParts.Count);
        }

        [Test]
        public void SplitAtPoundSigns_1111_Pound_2222_Ignores_Parts_Count_3()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("#");
            container.AddressParts.Add("2222");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtPoundSigns();

            // Assert
            Assert.AreEqual(3, container.AddressParts.Count);
        }

        [Test]
        public void SplitAtHyphens_1111Hyphen2222_All_2_Correct()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111-2222");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtHyphens();

            // Assert
            Assert.AreEqual(2, container.AddressParts.Count);
            Assert.AreEqual("1111", container.AddressParts[0]);
            Assert.AreEqual("2222", container.AddressParts[1]);
        }

        [Test]
        public void SplitAtHyphens_1111_2222Hyphen3333_All_3_Correct()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("2222-3333");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtHyphens();

            // Assert
            Assert.AreEqual(3, container.AddressParts.Count);
            Assert.AreEqual("1111", container.AddressParts[0]);
            Assert.AreEqual("2222", container.AddressParts[1]);
            Assert.AreEqual("3333", container.AddressParts[2]);
        }

        [Test]
        public void SplitAtHyphens_1111__Hyphen3333_Removes_Hyphen_All_2_Correct()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("-3333");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtHyphens();

            // Assert
            Assert.AreEqual(2, container.AddressParts.Count);
            Assert.AreEqual("1111", container.AddressParts[0]);
            Assert.AreEqual("3333", container.AddressParts[1]);
        }

        [Test]
        public void SplitAtHyphens_1111_Hyphen_2222_IgnoresHyphen()
        {
            // Arrange
            var container = new AddressParseContainer("fake input");
            container.AddressParts.Add("1111");
            container.AddressParts.Add("-");
            container.AddressParts.Add("2222");
            var splitter = new AddressSplitter(container);

            // Act
            splitter.SplitAtHyphens();

            // Assert
            Assert.AreEqual(2, container.AddressParts.Count);
            Assert.AreEqual("1111", container.AddressParts[0]);
            Assert.AreEqual("2222", container.AddressParts[1]);
        }

        [Test]
        public void ReplaceWhiteSpace_NonBreakingSpaceAndTab_ReplacedWithSpace()
        {
            // Arrange
            var container = new AddressParseContainer("1234\u00a0Main\tSt");
            var splitter = new AddressSplitter(container);

            Assert.AreEqual("1234\u00a0Main\tSt", container.CompleteAddressToParse);

            splitter.SplitAndClean();

            // Assert
            Assert.AreEqual("1234\u0020Main\u0020St", container.CompleteAddressToParse);
        }

        [Test]
        public void SplitCommasIntoGroups()
        {
            // Arrange
            var container = new AddressParseContainer("1234 Main St, Unit B1, Seattle, WA 98111");
            var splitter = new AddressSplitter(container);

            splitter.SplitAndClean();

            // Assert
            Assert.AreEqual("1234", container.AddressPartsGrouped[0][0]);
            Assert.AreEqual("Main", container.AddressPartsGrouped[0][1]);
            Assert.AreEqual("St", container.AddressPartsGrouped[0][2]);
            Assert.AreEqual("Unit", container.AddressPartsGrouped[1][0]);
            Assert.AreEqual("B1", container.AddressPartsGrouped[1][1]);
            Assert.AreEqual("Seattle", container.AddressPartsGrouped[2][0]);
            Assert.AreEqual("WA", container.AddressPartsGrouped[3][0]);
            Assert.AreEqual("98111", container.AddressPartsGrouped[3][1]);
        }
    }
}
