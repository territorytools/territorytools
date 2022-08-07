using System.Text.RegularExpressions;
using System.Collections.Generic;
using NUnit.Framework;
using TerritoryTools.Entities;
using TerritoryTools.Entities.AddressParsers;
using static TerritoryTools.Entities.Address;

namespace MinistryEntities.Tests
{
    [TestFixture]
    public class AddressParserTest
    {
        readonly IEnumerable<StreetType> StreetTypes 
            = new List<StreetType>() 
            { 
                new StreetType("STREET"){ Abbreviation = "ST" },
                new StreetType("ROAD"){ Abbreviation = "RD" },
                new StreetType("WAY"){ Abbreviation = "WY" },
                new StreetType("AVENUE"){ Abbreviation = "AVE" },
                new StreetType("VALLEY")
            };

        readonly IEnumerable<StreetType> StreetNameAfterTypes
            = new List<StreetType>()
            {
                new StreetType("HIGHWAY"){ Abbreviation = "HWY" }
            };

        [Test]
        public void Parse_Null_ThrowsExceptions()
        {
            Assert.Throws<AddressParsingException>(() => Parse(null));
        }

        [Test]
        public void Parse_Empty_ThrowsExceptions()
        {
            Assert.Throws<AddressParsingException>(() => Parse(string.Empty));
        }

        [Test]
        public void Parse_Blank_ThrowsExceptions()
        {
            Assert.Throws<AddressParsingException>(() => Parse(" "));
        }

        [Test]
        public void Parse_StreetNumber()
        {
            var address = Parse("1234 NE Main St Apt 100");

            Assert.AreEqual("1234", address.Number);
        }

        [Test]
        public void Parse_StreetDirection()
        {
            var address = Parse("1234 NE Main St Apt 100");

            Assert.AreEqual("NE", address.DirectionalPrefix);
        }

        [Test]
        public void Parse_StreetDirection_Commas()
        {
            var address = Parse("1234 NE Main St, Apt 100");

            Assert.AreEqual("NE", address.DirectionalPrefix);
        }

        [Test]
        public void Parse_UnitType_Commas()
        {
            var address = Parse("1234 NE Main St, Apt 100");

            Assert.AreEqual("Apt", address.UnitType);
        }

        [Test]
        public void Parse_StreetType_Commas()
        {
            var address = Parse("1234 NE Main St, Apt 100");

            Assert.AreEqual("St", address.StreetType);
        }

        [Test]
        public void Parse_StreetName_IsNumber()
        {
            var address = Parse("1234 100th St");

            Assert.AreEqual("100th", address.StreetName);
        }

        [Test]
        public void Parse_StreetName_IsNumber_Long()
        {
            var address = Parse("1234 NE 100th St Apt A-20");

            Assert.AreEqual("100th", address.StreetName);
        }

        [Test]
        public void Parse_UnitName_HasHyphen()
        {
            var address = Parse("1234 NE 100th St Apt A-20");

            Assert.AreEqual("A-20", address.UnitNumber);
        }

        [Test]
        public void Parse_UnitNumber_WithLetterPrefix()
        {
            var address = Parse("1234 NE Main St Apt X100");

            Assert.AreEqual("X100", address.UnitNumber);
        }

        [Test]
        public void Parse_StreetName_1234_Juniper_LN()
        {
            var address = Parse("1234 Juniper LN");

            Assert.AreEqual("Juniper", address.StreetName);
        }

        [Test]
        public void Parse_StreetName_1234_Juniper_St()
        {
            var address = Parse("1234 Juniper St");

            Assert.AreEqual("Juniper", address.StreetName);
        }

        [Test]
        public void Parse_StreetName_1234_Juniper_St_Apt_5()
        {
            var address = Parse("1234 Juniper St Apt 5");

            Assert.AreEqual("Juniper", address.StreetName);
        }

        [Test]
        public void Parse_UnitType_WithJustPoundSign()
        {
            var address = Parse("1234 Juniper St #5");

            Assert.AreEqual("#", address.UnitType);
        }

        [Test]
        public void Parse_UnitNumber_WithJustPoundSign()
        {
            var address = Parse("1234 Juniper St #5");

            Assert.AreEqual("5", address.UnitNumber);
        }


        [Test]
        public void Parse_StreetName_1234_NE_Juniper_St_Apt_5()
        {
            var address = Parse("1234 NE Juniper St Apt 5");

            Assert.AreEqual("Juniper", address.StreetName);
        }

        [Test]
        public void Parse_UnitType_UnitNumber()
        {
            var address = Parse("1234 NE Main St Apt 100");

            Assert.AreEqual("Apt", address.UnitType, "UnitType");
            Assert.AreEqual("100", address.UnitNumber, "UnitNumber");
        }


        [Test]
        public void Parse_UnitNumber_Simple()
        {
            var address = Parse("1234 NE Main St Apt 100");

            Assert.AreEqual("100", address.UnitNumber);
        }

        [Test]
        public void Parse_UnitNumberB()
        {
            var address = Parse("1234 NE Main St Apt B");

            Assert.AreEqual("B", address.UnitNumber);
        }

        [Test]
        public void Parse_Short_AddsParsingError_MissingStreetType()
        {
            Assert.Throws<MissingStreetTypeException>(
                code: () => Parse("1234 Portland Apt 100"),
                message: "Missing Street Type: 1234 Portland Apt 100");
        }

        [Test]
        public void Parse_StreetName_6Words()
        {
            var address = Parse("1234 NE Main St Apt 100");

            Assert.AreEqual("Main", address.StreetName);
        }

        [Test]
        public void Parse_DoubleSpace_UnitTypeNumber()
        {
            var door = Parse("1234  Juniper St Apt  100");

            Assert.AreEqual("Apt", door.UnitType);
            Assert.AreEqual("100", door.UnitNumber);
        }

        [Test]
        public void Parse_StreetDirectionPrefix_1234_NE_Juniper_Street_Apt_100()
        {
            var door = Parse("1234 NE Juniper Street Apt 100");

            Assert.AreEqual("NE", door.DirectionalPrefix);
        }

        [Test]
        public void Parse_StreetDirectionPrefix_Full()
        {
            var door = Parse("1234 North Juniper Ln Apt 100");

            Assert.AreEqual("North", door.DirectionalPrefix);
        }

        [Test]
        public void Parse_StreetName_TwoWords()
        {
            var door = Parse("1234 North Columbia Park St Apt 100");

            Assert.AreEqual("Columbia Park", door.StreetName);
        }

        [Test]
        public void Parse_StreetType_SimpleStreet()
        {
            var door = Parse("1234 North Juniper St Apt 100");

            Assert.AreEqual("St", door.StreetType);
        }

        [Test]
        public void Parse_StreetDirectionSuffix_S()
        {
            var door = Parse("1234 Juniper St S Apt 100");

            Assert.AreEqual("S", door.DirectionalSuffix);
        }

        [Test]
        public void Parse_TrailingSpace_UnitNumber_Removed()
        {
            var door = Parse("1234 Juniper Ave Apt 100  ");

            Assert.AreEqual("Apt", door.UnitType, "UnitType");
            Assert.AreEqual("100", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_1_Juniper_Ave()
        {
            var door = Parse("1 Juniper Ave");

            Assert.AreEqual("1", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_20_Juniper_Tree_Ave()
        {
            var door = Parse("20 Juniper Tree Ave");

            Assert.AreEqual("20", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper Tree", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_StreenName_HasHyphen()
        {
            var door = Parse("20 Juniper-Tree Ave");

            Assert.AreEqual("20", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper-Tree", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_300_N_Juniper_Tree_Ave()
        {
            var door = Parse("300 N Juniper Tree Ave");

            Assert.AreEqual("300", door.Number, "StreetNumber");
            Assert.AreEqual("N", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper Tree", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_4000_N_Juniper_Tree_Ave_Apt_1()
        {
            var door = Parse("4000 N Juniper Tree Ave Apt 1");

            Assert.AreEqual("4000", door.Number, "StreetNumber");
            Assert.AreEqual("N", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper Tree", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("Apt", door.UnitType, "UnitType");
            Assert.AreEqual("1", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_5555_Juniper_Ave_Unit_20()
        {
            var door = Parse("5555 Juniper Ave Unit 20");

            Assert.AreEqual("5555", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Juniper", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("Unit", door.UnitType, "UnitType");
            Assert.AreEqual("20", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_UnitNumber_NoUnitType_AlphaNumeric_5555_777TH_Ave_AB20()
        {
            var door = Parse("5555 777th Ave AB20");



            Assert.AreEqual("5555", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("777th", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("AB20", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_UnitNumber_AlphaNumeric_5555_777TH_Ave_Unit_AB20()
        {
            var door = Parse("5555 777th Ave Unit AB20");

            Assert.AreEqual("5555", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("777th", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("Unit", door.UnitType, "UnitType");
            Assert.AreEqual("AB20", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_5555_777TH_Ave_Unit_20()
        {
            var door = Parse("5555 777th Ave Unit 20");

            Assert.AreEqual("5555", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("777th", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("Unit", door.UnitType, "UnitType");
            Assert.AreEqual("20", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_AddressReal1()
        {
            var door = Parse("1347 15th Ave S #1");

            Assert.AreEqual("1347", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("#", door.UnitType, "UnitType");
            Assert.AreEqual("1", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("15th", door.StreetName, "StreetName");
        }

        [Test]
        public void Parse_AddressReal2()
        {
            var door = Parse("1347 15th Ave S");

            Assert.AreEqual("1347", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("15th", door.StreetName, "StreetName");
        }

        [Test]
        public void Parse_2Addresses()
        {
            var doorA = Parse("1347 15th Ave S");

            Assert.AreEqual("1347", doorA.Number);
            Assert.AreEqual("", doorA.DirectionalPrefix);
            Assert.AreEqual("Ave", doorA.StreetType);
            Assert.AreEqual("S", doorA.DirectionalSuffix);
            Assert.AreEqual("", doorA.UnitType);
            Assert.AreEqual("", doorA.UnitNumber);
            Assert.AreEqual("15th", doorA.StreetName, "StreetName");

            var doorB = Parse("1348 16th Ave S");

            Assert.AreEqual("1348", doorB.Number);
            Assert.AreEqual("", doorB.DirectionalPrefix);
            Assert.AreEqual("Ave", doorB.StreetType);
            Assert.AreEqual("S", doorB.DirectionalSuffix);
            Assert.AreEqual("", doorB.UnitType);
            Assert.AreEqual("", doorB.UnitNumber);
            Assert.AreEqual("16th", doorB.StreetName, "StreetName");
        }

        [Test]
        [Ignore("Not working yet")]
        public void Parse_Hwy_99()
        {
            var door = Parse("1234 Hwy 99");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Hwy", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("99", door.StreetName, "StreetName");
            Assert.IsTrue(door.StreetNameIsAfterType, "StreetNameIsAfterType");
            Assert.AreEqual("123 Hwy 99", door.CombineStreet(), "CombineStreet");
        }

        [Test]
        public void Parse_1234_A_St()
        {
            var door = Parse("1234 A St");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("A", door.StreetName, "StreetName");
        }

        [Test]
        public void Parse_1234_A_St_S()
        {
            var door = Parse("1234 A St S");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("A", door.StreetName, "StreetName");
        }

        [Test]
        public void TestEmptyStringOn_IsNullOrWhiteSpace()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(""));
        }


        [Test]
        public void Parse_ComplexStreetName_FindsStreetType()
        {
            var door = Parse("8428 M L King Jr Way S");

            // Main thing to find
            Assert.AreEqual("Way", door.StreetType, "StreetType");

            // Other things
            Assert.AreEqual("8428", door.Number, "StreetNumber");
            Assert.AreEqual("", door.NumberFraction, "StreetNumberFraction");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("M L King Jr", door.StreetName, "StreetName");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("", door.City, "City");
            Assert.AreEqual("", door.State, "State");
            Assert.AreEqual("", door.PostalCode, "Zip");
            Assert.AreEqual("", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_1234B_A_St_S_Seattle_WA_98144_1234()
        {
            var door = Parse("1234B A St S Seattle, WA  98144-2345");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("B", door.NumberFraction, "StreetNumberFraction");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("A", door.StreetName, "StreetName");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Seattle", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98144", door.PostalCode, "Zip");
            Assert.AreEqual("2345", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_18125_256th()
        {
            // Valley is recognized as a StreetType, so this is currently the only way I know
            var door = Parse(
                "18125 256th Avenue Southeast", 
                city: "Maple Valley", 
                state: "WA", 
                postalCode: "98038");

            Assert.AreEqual("18125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("256th", door.StreetName, "StreetName");
            Assert.AreEqual("Avenue", door.StreetType, "StreetType");
            Assert.AreEqual("Southeast", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Maple Valley", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98038", door.PostalCode, "Zip");
            Assert.AreEqual("", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Normalize_18125_256th()
        {
            var door = Normalize(
                "18125 256th Avenue Southeast, , ", 
                city: "Maple Valley", 
                state: "WA", 
                postalCode: "98038");

            Assert.AreEqual("18125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("256th", door.StreetName, "StreetName");
            Assert.AreEqual("Ave", door.StreetType, "StreetType");
            Assert.AreEqual("SE", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Maple Valley", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98038", door.PostalCode, "Zip");
            Assert.AreEqual("", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_10237_SE_3rd_St_Apt_4_Bellevue_WA_98004()
        {
            var door = Parse("10237 SE 3rd St Apt 4 Bellevue WA  98004");

            Assert.AreEqual("10237", door.Number, "StreetNumber");
            Assert.AreEqual("SE", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("3rd", door.StreetName, "StreetName");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("Apt", door.UnitType, "UnitType");
            Assert.AreEqual("4", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Bellevue", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98004", door.PostalCode, "Zip");
            Assert.AreEqual("", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_1234_A_St_S_Seattle_WA_98144_2345()
        {
            var door = Parse("1234 A St S Seattle, WA  98144-2345");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("A", door.StreetName, "StreetName");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Seattle", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98144", door.PostalCode, "Zip");
            Assert.AreEqual("2345", door.PostalCodeExt, "ZipExt");

        }

        [Test]
        public void Parse_CityHasMultipleWords_1234B_A_St_S_Federal_Way_WA_98144_1234()
        {
            // TODO: Identify the state at the end, but now this is the only 
            // way I know how to find a city with a street type name in it.
            var door = Parse(
                "1234B A St S", 
                city: "Federal Way", 
                state: "WA", 
                postalCode: "98144-2345");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("B", door.NumberFraction, "StreetNumberFraction");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("A", door.StreetName, "StreetName");
            Assert.AreEqual("Federal Way", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98144", door.PostalCode, "Zip");
            Assert.AreEqual("2345", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_CityHasMultipleWords_1234_onehalf_A_St_S_Federal_Way_WA_98144_1234()
        {
            var door = Parse("1234 1/2 A St S", city: "Federal Way", state: "WA", postalCode:  "98144-2345");


            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("1/2", door.NumberFraction, "StreetNumberFraction");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("A", door.StreetName, "StreetName");
            Assert.AreEqual("Federal Way", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98144", door.PostalCode, "Zip");
            Assert.AreEqual("2345", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void Parse_CityHasMultipleWords_1234_A_St_S_Federal_Way_WA_98144_1234()
        {
            var door = Parse("1234 A St S", city: "Federal Way", state: "WA", postalCode:  "98144-2345");

            Assert.AreEqual("1234", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("S", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("A", door.StreetName, "StreetName");
            Assert.AreEqual("Federal Way", door.City, "City");
            Assert.AreEqual("WA", door.State, "State");
            Assert.AreEqual("98144", door.PostalCode, "Zip");
            Assert.AreEqual("2345", door.PostalCodeExt, "ZipExt");
        }

        [Test]
        public void SameAddressAs_5555Portland_True()
        {
            AssertSame("5555 Portland St", "5555 Portland St");
        }

        [Test]
        public void SameAddressAs_StreetNumberDifferent_False()
        {
            AssertNotSame("6666 Portland St", "5555 Portland St");
        }


        [Test]
        public void SameAddressAs_StreetNameDifferent_False()
        {
            AssertNotSame("5555 Different St", "5555 Portland St");
        }

        [Test]
        public void SameAddressAs_ThisCityEmpty_True()
        {
            var door = Parse("5555 Portland St");
            door.City = string.Empty;

            var other = Parse("5555 Portland St");
            other.City = "Seattle";

            Assert.IsTrue(door.SameAs(other, SameAsOptions.AssumeMissingCityIsSame));
        }

        [Test]
        public void SameAddressAs_OtherCityEmpty_True()
        {
            var door = Parse("5555 Portland St");
            door.City = "Seattle";

            var other = Parse("5555 Portland St");
            other.City = string.Empty;

            Assert.IsFalse(door.SameAs(other));
        }

        [Test]
        public void SameAddressAs_OtherCityNull_Assume_True()
        {
            var door = Parse("5555 Portland St");
            door.City = "Seattle";

            var other = Parse("5555 Portland St");
            other.City = null;

            Assert.IsTrue(door.SameAs(other, SameAsOptions.AssumeMissingCityIsSame));
        }

        [Test]
        public void SameAddressAs_WithSameUnits_True()
        {
            var door = Parse("5555 Portland St Unit 5");
            door.City = "Seattle";

            var other = Parse("5555 Portland St Apt 5");
            other.City = "Seattle";

            Assert.IsTrue(door.SameAs(other));
        }

        [Test]
        public void SameAddressAs_WithDifferentUnits_False()
        {
            var door = Parse("5555 Portland St Unit 5");
            door.City = "Seattle";

            var other = Parse("5555 Portland St Apt 6");
            other.City = "Seattle";

            Assert.IsFalse(door.SameAs(other));
        }

        [Test]
        public void Parse_125_Main_St_comma_Richmond_VA_23221()
        {
            var door = Parse("125 Main St, Richmond VA 23221");

            Assert.AreEqual("125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Main", door.StreetName, "StreetName");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
            Assert.AreEqual("Richmond", door.City, "City");
            Assert.AreEqual("VA", door.State, "State");
            Assert.AreEqual("23221", door.PostalCode, "Zip");
        }

        [Test]
        public void Parse_125_Main_St()
        {
            var door = Parse("125 Main St");

            Assert.AreEqual("125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Main", door.StreetName, "StreetName");
            Assert.AreEqual("St", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_123_Broadway_NoStreetType()
        {
            var door = Parse("125 Broadway");

            Assert.AreEqual("125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Broadway", door.StreetName, "StreetName");
            Assert.AreEqual("", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("", door.UnitNumber, "UnitNumber");
        }


        [Test]
        public void Parse_123_Broadway_withCommaThenUnit_NoStreetType()
        {
            var door = Parse("125 Broadway 2A"); // Not a directional or a street type

            Assert.AreEqual("125", door.Number, "StreetNumber");
            Assert.AreEqual("", door.DirectionalPrefix, "StreetDirectionalPrefix");
            Assert.AreEqual("Broadway", door.StreetName, "StreetName");
            Assert.AreEqual("", door.StreetType, "StreetType");
            Assert.AreEqual("", door.DirectionalSuffix, "StreetDirectionSuffix");
            Assert.AreEqual("", door.UnitType, "UnitType");
            Assert.AreEqual("2A", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void Parse_UnitNumber_MovesPoundSign()
        {
            var door = Parse("325 9th Ave #359819");

            Assert.AreEqual("#", door.UnitType, "UnitType");
            Assert.AreEqual("359819", door.UnitNumber, "UnitNumber");
        }

        [Test]
        public void SameAs_StreetType_Abbreviations_True()
        {
            AssertSame("100 MAIN ST", "100 MAIN STREET");
        }

        [Test]
        public void Normalize_StreetType_BecomesAbbreviation()
        {
            var door = Normalize("100 MAIN STREET");

            Assert.AreEqual("St", door.StreetType, "StreetType");
        }

        [Test]
        public void Normalize_StreetType_BecomesTitleCase()
        {
            var door = Normalize("100 MAIN ST");

            Assert.AreEqual("St", door.StreetType, "StreetType");
        }

        [Test]
        public void Parse_NumericSuffix_BecomesLowerCase()
        {
            var door = Normalize("100 1ST ST");

            Assert.AreEqual("1st", door.StreetName, "StreetName");
        }

        [Test]
        public void RegexReplaceTest()
        {
            var input = "1ST 102Nd 33rD";
            var pattern = @"((\d+)(st|nd|rd|th))";
            var evaluator = new MatchEvaluator((m) => m.Captures[0].Value.ToLower());
            var actual = Regex.Replace(input, pattern, evaluator, RegexOptions.IgnoreCase);

            Assert.AreEqual("1st 102nd 33rd", actual);
        }

        [Test]
        public void RegexReplace_State_Test()
        {
            var input = "1ST 102Nd 33rD";
            var pattern = @"((\d+)(st|nd|rd|th))";
            var evaluator = new MatchEvaluator((m) => m.Captures[0].Value.ToLower());
            var actual = Regex.Replace(input, pattern, evaluator, RegexOptions.IgnoreCase);

            Assert.AreEqual("1st 102nd 33rd", actual);
        }

        [Test]
        public void Parse_ComplexAddress_ThrowsMissingStreetType()
        {
            Assert.Throws<MissingStreetTypeException>(
                code: () => Parse(@"523 Broadway E Apt 664, Seattle WA 98102-5381"),
                message: "Missing Street Type: 523 Broadway E Apt 664, Seattle WA 98102-5381");
        }

        void AssertSame(
            string firstAddress, 
            string firstCity, 
            string secondAddress, 
            string secondCity)
        {
            var first = Parse(firstAddress);
            first.City = firstCity;

            var second = Parse(secondAddress);
            second.City = secondCity;

            Assert.IsTrue(first.SameAs(second));
        }

        void AssertSame(
            string a, 
            string b, 
            SameAsOptions options = SameAsOptions.None)
        {
            var first = Normalize(a);
            var second = Normalize(b);

            Assert.IsTrue(first.SameAs(second, options));
        }

        void AssertNotSame(
                    string firstAddress,
                    string firstCity,
                    string secondAddress,
                    string secondCity)
        {
            var first = Parse(firstAddress);
            first.City = firstCity;

            var second = Parse(secondAddress);
            second.City = secondCity;

            Assert.IsTrue(first.SameAs(second, SameAsOptions.AssumeMissingCityIsSame));
        }

        void AssertNotSame(string firstAddress, string secondAddress)
        {
            var first = Parse(firstAddress);
            var second = Parse(secondAddress);

            Assert.IsFalse(first.SameAs(second));
        }

        Address Parse(
            string text,
            string city = null,
            string state = null,
            string postalCode = null)
        {
            return new CompleteAddressParser(StreetTypes, StreetNameAfterTypes)
               .Parse(text, city, state, postalCode);
        }

        Address Normalize(
            string text,
            string city = null,
            string state = null,
            string postalCode = null)
        {
            return new CompleteAddressParser(StreetTypes, StreetNameAfterTypes)
               .Normalize(text, city, state, postalCode);
        }
    }
}
