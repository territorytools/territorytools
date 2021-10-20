using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Controllers.Tests.AlbaBackupToS13
{
    public class S13EntryConverterTests
    {
        public IList<AssignmentChange> LoadSeedValues()
        {
            List<AssignmentValues> values = AssignmentValues
                .LoadFromCsv("AlbaBackupToS13/1900-01-01_000000/territories.txt");

            List<AssignmentChange> changes = AssignmentChange.Load(values);

            return changes;
        }

        [Test]
        public void GivenTwoValueEntriesSameNameInThenOut_ShouldBeOneS13Entry()
        {
            List<S13Entry> entries = S13EntryConverter
                .Convert(LoadSeedValues());

            var entry = entries[0];

            Assert.AreEqual("Bruce Wayne", entry.Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), entry.CheckedOut);
            Assert.AreEqual(DateTime.Parse("2001-02-01"), entry.CheckedIn);
        }

        [Test]
        public void GivenDifferentPublisherNameInOut_ShouldBeLastPublisher()
        {
            List<S13Entry> entries = S13EntryConverter
                .Convert(LoadSeedValues());
            
            var entry = entries[1];

            Assert.AreEqual("Superman", entry.Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), entry.CheckedOut);
            Assert.AreEqual(DateTime.Parse("2002-03-02"), entry.CheckedIn);

            var entry2 = entries[2];

            Assert.AreEqual("Clark Kent", entry2.Publisher);
            Assert.AreEqual(DateTime.Parse("2002-05-05"), entry2.CheckedOut);
            Assert.AreEqual(null, entry2.CheckedIn);
        }

        [Test]
        public void GivenInThenOut_ShouldAddTwo()
        {
            List<S13Entry> entries = S13EntryConverter
                .Convert(LoadSeedValues());

            var entry = entries[3];

            Assert.AreEqual("Lana Lang", entry.Publisher);
            Assert.AreEqual(null, entry.CheckedOut);
            Assert.AreEqual(DateTime.Parse("2003-01-01"), entry.CheckedIn);

            var entry2 = entries[4];

            Assert.AreEqual("Lana Lang", entry2.Publisher);
            Assert.AreEqual(DateTime.Parse("2003-02-02"), entry2.CheckedOut);
            Assert.AreEqual(null, entry2.CheckedIn);
        }

        [Test]
        public void CompareExpectedCsvToActualResults()
        {
            List<S13Entry> actuals = S13EntryConverter
               .Convert(LoadSeedValues());

            List <S13Entry> expecteds = S13Entry.LoadCsv("AlbaBackupToS13/expected.csv");

            Assert.AreEqual(expecteds.Count, actuals.Count);
            for(int i = 0; i < expecteds.Count; i++)
            {
                var expected = expecteds[i];
                var actual = actuals[i];
                Assert.AreEqual(expected.Number, actual.Number);
                Assert.AreEqual(expected.Publisher, actual.Publisher);
                Assert.AreEqual(expected.CheckedOut, actual.CheckedOut);
                Assert.AreEqual(expected.CheckedIn, actual.CheckedIn);
            }
        }
    }
}
