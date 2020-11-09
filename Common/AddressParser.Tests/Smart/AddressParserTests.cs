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

        [Test]
        public void Parser_PO_Box()
        {
            AssertStreetNumberName("PO Box 321", "321", "PO Box");
        }

        [Test]
        public void Parser_PO_Box_wDots()
        {

            AssertStreetNumberName("P.O. Box 321", "321", "P.O. Box");
        }

        [Test]
        public void Parser_POBox_wDots()
        {

            AssertStreetNumberName("P.O.Box 321", "321", "P.O.Box");
        }

        [Test]
        public void Parser_P_O_Box_wDotsSpaces()
        {

            AssertStreetNumberName("P. O. Box 321", "321", "P. O. Box");
        }

        [Test]
        public void Parser_POB()
        {

            AssertStreetNumberName("POB 321", "321", "POB");
        }

        [Test]
        public void Parser_POB_wSpaces()
        {

            AssertStreetNumberName("P O B 321", "321", "P O B");
        }

        [Test]
        public void Parser_P_O_B_wDotsSpaces()
        {

            AssertStreetNumberName("P. O. B. 321", "321", "P. O. B.");
        }

        [Test]
        public void Parser_Post_Office_Box()
        {

            AssertStreetNumberName("Post Office Box 321", "321", "Post Office Box");
        }

        [Test]
        public void Parser_Post_Office_Barn_Fails()
        {

            AssertStreetNumberName("Post Office Barn 321", string.Empty, string.Empty);
        }

        [Test]
        public void Parser_Region_Code_Normal()
        {
            Assert.AreEqual("WA", Test("123 Main St Lynnwood WA 98087").Region.Code.ToString());
        }

        [Test]
        public void Parser_Postal_Code_Normal()
        {
            Assert.AreEqual("98087", Test("123 Main St Lynnwood WA 98087").Postal.Code.ToString());
        }

        Address Test(string text)
        {
            var parser = new Parser();

            return parser.Parse(text);
        }

        void AssertStreetNumberName(string text, string streetNumber, string streetName)
        {
            Assert.AreEqual(streetName, Test(text).Street.Name.Name.ToString());
            Assert.AreEqual(streetNumber, Test(text).Street.Number.ToString());
        }
    }
}
