using System;
using NUnit.Framework;
using TerritoryTools.Entities.AddressParsers;
using System.Collections.Generic;

namespace MinistryEntities.Tests.Parsers
{
    [TestFixture]
    public class AddressPartResultExtensionsTest
    {
        AddressPartResult zero = new AddressPartResult()
        {
            Index = 0,
            Value = "anything",
        };

        AddressPartResult one = new AddressPartResult()
        {
            Index = 1,
            Value = "anything",
        };

        AddressPartResult two = new AddressPartResult()
        {
            Index = 2,
            Value = "anything",
        };

        AddressPartResult three = new AddressPartResult()
        {
            Index = 3,
            Value = "anything",
        };


        [Test]
        public void One_IsAfter_Zero_True()
        {
            Assert.IsTrue(one.IsAfter(zero));
        }

        [Test]
        public void One_IsAfter_Two_False()
        {
            Assert.IsFalse(one.IsAfter(two));
        }

        [Test]
        public void One_IsAfter_One_False()
        {
            Assert.IsFalse(one.IsAfter(one));
        }

        [Test]
        public void Two_IsAfter_Zero_True()
        {
            Assert.IsTrue(two.IsAfter(zero));
        }

        [Test]
        public void One_IsAtLeastThisManyBefore_One_0_True()
        {
            Assert.IsTrue(one.IsAtLeastNBefore(0, one));
        }

        [Test]
        public void Zero_IsAtLeastThisManyBefore_Two_0_True()
        {
            Assert.IsTrue(zero.IsAtLeastNBefore(0, two));
        }

        [Test]
        public void Zero_IsAtLeastThisManyBefore_Two_1_True()
        {
            Assert.IsTrue(zero.IsAtLeastNBefore(1, two));
        }

        [Test]
        public void Zero_IsAtLeastThisManyBefore_Two_3_False()
        {
            Assert.IsFalse(zero.IsAtLeastNBefore(3, two));
        }


        [Test]
        public void Three_IsAtLeastThisManyBefore_Two_0_False()
        {
            Assert.IsFalse(three.IsAtLeastNBefore(0, two));
        }

        [Test]
        public void One_IsAtLeastThisManyAfter_One_0_True()
        {
            Assert.IsTrue(one.IsAtLeastNAfter(0, one));
        }

        [Test]
        public void Two_IsAtLeastThisManyAfter_Zero_0_True()
        {
            Assert.IsTrue(two.IsAtLeastNAfter(0, zero));
        }

        [Test]
        public void Two_IsAtLeastThisManyAfter_Zero_1_True()
        {
            Assert.IsTrue(two.IsAtLeastNAfter(1, zero));
        }

        [Test]
        public void Two_IsAtLeastThisManyAfter_Zero_2_True()
        {
            Assert.IsTrue(two.IsAtLeastNAfter(2, zero));
        }

        [Test]
        public void Two_IsAtLeastThisManyAfter_Zero_3_False()
        {
            Assert.IsFalse(two.IsAtLeastNAfter(3, zero));
        }


        [Test]
        public void Two_IsAtLeastThisManyAfter_Three_0_False()
        {
            Assert.IsFalse(two.IsAtLeastNAfter(0, three));
        }


        [Test]
        public void Zero_GetItemAfterMeIn_List_Returns_One()
        {
            var list = GetListZeroThroughThree();

            var actual = zero.GetItemAfterMeIn(list);

            Assert.IsTrue(actual == one);
        }


        [Test]
        public void One_GetItemAfterMeIn_List_Returns_Two()
        {
            var list = GetListZeroThroughThree();

            var actual = one.GetItemAfterMeIn(list);

            Assert.IsTrue(actual == two);
        }

        [Test]
        public void Three_GetItemAfterMeIn_List_Returns_Null()
        {
            var list = GetListZeroThroughThree();

            var actual = three.GetItemAfterMeIn(list);

            Assert.IsNull(actual);
        }

        [Test]
        public void Three_IsLastItemIn_List_True()
        {
            var list = GetListZeroThroughThree();

            var actual = three.IsLastItemIn(list);

            Assert.IsTrue(actual);
        }

        [Test]
        public void Two_IsLastItemIn_List_False()
        {
            var list = GetListZeroThroughThree();

            var actual = two.IsLastItemIn(list);

            Assert.IsFalse(actual);
        }

        private List<AddressPartResult> GetListZeroThroughThree()
        {
            var list = new List<AddressPartResult>()
            {
                zero,
                one,
                two,
                three,
            };
            return list;
        }
    }
}
