using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class AddressSameAs
    {
        [Test]
        public void SameAs_Simple()
        {
            var first = new Address();
            first.Street.Number = "123";
            first.Street.Name.Name = "Main";
            first.Street.Name.StreetType = "St";
            first.City.Name = "Bellevue";
            first.Region.Code = "WA";
            first.Postal.Code = "98001";

            var second = new Address();
            second.Street.Number = "123";
            second.Street.Name.Name = "Main";
            second.Street.Name.StreetType = "St";
            second.City.Name = "Bellevue";
            second.Region.Code = "WA";
            second.Postal.Code = "98001";

            Assert.IsTrue(first.SameAs(second));

        }

        [Test]
        public void SameAs_NoStreetType()
        {
            var first = new Address();
            first.Street.Number = "123";
            first.Street.Name.Name = "Broadway";
            first.City.Name = "Bellevue";
            first.Region.Code = "WA";
            first.Postal.Code = "98001";

            var second = new Address();
            second.Street.Number = "123";
            second.Street.Name.Name = "Broadway";
            second.City.Name = "Bellevue";
            second.Region.Code = "WA";
            second.Postal.Code = "98001";

            Assert.IsTrue(first.SameAs(second));
        }

        [Test]
        public void SameAs_NonStreetType()
        {
            var first = new Address();
            first.Street.Number = "123";
            first.Street.Name.NamePrefix = "PO Box";
            first.City.Name = "Bellevue";
            first.Region.Code = "WA";
            first.Postal.Code = "98001";

            var second = new Address();
            second.Street.Number = "123";
            second.Street.Name.NamePrefix = "PO Box";
            second.City.Name = "Bellevue";
            second.Region.Code = "WA";
            second.Postal.Code = "98001";

            Assert.IsTrue(first.SameAs(second));
        }

        [Test]
        [Ignore("Not working yet")]
        public void SameAs_NonStreetType2()
        {
            var first = new Address();
            first.Street.Number = "123";
            first.Street.Name.NamePrefix = "PO Box";
            first.City.Name = "Bellevue";
            first.Region.Code = "WA";
            first.Postal.Code = "98001";

            var second = new Address();
            second.Street.Number = "123";
            second.Street.Name.NamePrefix = "POB";
            second.City.Name = "Bellevue";
            second.Region.Code = "WA";
            second.Postal.Code = "98001";

            Assert.IsTrue(first.SameAs(second));
        }
    }
}
