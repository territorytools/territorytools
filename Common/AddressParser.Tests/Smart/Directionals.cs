﻿using NUnit.Framework;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    public class Directionals : ParserTestBase
    {
        [TestCase("123 N Main St Lynnwood WA 98087", "N")]
        [TestCase("123 S Main St Lynnwood WA 98087", "S")]
        [TestCase("123 W Main St Lynnwood WA 98087", "W")]
        [TestCase("123 E Main St Lynnwood WA 98087", "E")]
        [TestCase("123 SW Main St Lynnwood WA 98087", "SW")]
        [TestCase("123 SE Main St Lynnwood WA 98087", "SE")]
        [TestCase("123 NE Main St Lynnwood WA 98087", "NE")]
        [TestCase("123 NW Main St Lynnwood WA 98087", "NW")]
        [TestCase("123 North Main St Lynnwood WA 98087", "North")]
        [TestCase("123 South Main St Lynnwood WA 98087", "South")]
        [TestCase("123 East Main St Lynnwood WA 98087", "East")]
        [TestCase("123 West Main St Lynnwood WA 98087", "West")]
        [TestCase("123 Northeast Main St Lynnwood WA 98087", "Northeast")]
        [TestCase("123 Northwest Main St Lynnwood WA 98087", "Northwest")]
        [TestCase("123 Southeast Main St Lynnwood WA 98087", "Southeast")]
        [TestCase("123 Southwest Main St Lynnwood WA 98087", "Southwest")]
        [TestCase("123 SE Mount Baker St Lynnwood WA 98087", "SE")]
        public void DirectionalPrefix(
            string text,
            string directionalPrefix)
        {
            Assert.AreEqual(directionalPrefix, Test(text).Street.Name.DirectionalPrefix);
        }

        [TestCase("123 Main St N Lynnwood WA 98087", "N")]
        [TestCase("123 Main St S Lynnwood WA 98087", "S")]
        [TestCase("123 Main St W Lynnwood WA 98087", "W")]
        [TestCase("123 Main St E Lynnwood WA 98087", "E")]
        [TestCase("123 Main St SW Lynnwood WA 98087", "SW")]
        [TestCase("123 Main St SE Lynnwood WA 98087", "SE")]
        [TestCase("123 Main St NE Lynnwood WA 98087", "NE")]
        [TestCase("123 Main St NW Lynnwood WA 98087", "NW")]
        [TestCase("123 Main St North Lynnwood WA 98087", "North")]
        [TestCase("123 Main St South Lynnwood WA 98087", "South")]
        [TestCase("123 Main St East Lynnwood WA 98087", "East")]
        [TestCase("123 Main St West Lynnwood WA 98087", "West")]
        [TestCase("123 Main St Northeast Lynnwood WA 98087", "Northeast")]
        [TestCase("123 Main St Northwest Lynnwood WA 98087", "Northwest")]
        [TestCase("123 Main St Southeast Lynnwood WA 98087", "Southeast")]
        [TestCase("123 Main St Southwest Lynnwood WA 98087", "Southwest")]
        public void DirectionalSuffix(string text, string directionalSuffx)
        {
            Assert.AreEqual(directionalSuffx, Test(text).Street.Name.DirectionalSuffix);
        }

        [Test]
        public void SuffixWinsOverShortPrefix()
        {
            AssertParts(
                text: "1234 W Lake Sammamish Pkwy SE Seattle WA 98123",
                streetNumber: "1234",
                dirPrefix: "",
                streetName: "W Lake Sammamish",
                streetType: "Pkwy",
                dirSuffix: "SE",
                city: "Seattle",
                region: "WA",
                postal: "98123");
        }

        [Test]
        public void SuffixWinsOverFullPrefix()
        {
            AssertParts(
                text: "1234 West Blue Mountain Rd SE Seattle WA 98123",
                streetNumber: "1234",
                dirPrefix: "",
                streetName: "West Blue Mountain",
                streetType: "Rd",
                dirSuffix: "SE",
                city: "Seattle",
                region: "WA",
                postal: "98123");
        }

        [Test]
        public void StreetType_WithDirectionalPrefixAndCityEtc()
        {
            Assert.AreEqual("St", Test("123 NE Main St Lynnwood WA 98087").Street.Name.StreetType);
        }

        [Test]
        public void StreetType_WithDirectionalSuffixAndCityEtc()
        {
            Assert.AreEqual("St", Test("123 Main St NE Lynnwood WA 98087").Street.Name.StreetType);
        }
    }
}
