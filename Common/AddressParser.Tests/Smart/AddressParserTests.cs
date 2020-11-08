using NUnit.Framework;
using TerritoryTools.Common.AddressParser.Smart;

namespace TerritoryTools.Common.AddressParser.Tests.Smart
{
    [TestFixture]
    public class AddressParserTests
    {
        [Test]
        public void Parser_City_Name_Null()
        {
            Assert.IsNull(Test(null).City.Name);
        }

        [Test]
        public void Parser_Street_Number_Normal()
        {
            Assert.AreEqual("123", Test("123 Main St").Street.Number.ToString());
        }

        Address Test(string text)
        {
            var parser = new Parser();

            return parser.Parse(text);
        }
    }
}
