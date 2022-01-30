using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Controllers.Tests.AlbaBackupToS13
{
    public class AssignmentChangeTests
    {
        public IList<AssignmentChange> LoadChanges()
        {
            IList<AssignmentValues> values = LoadValues();

            List<AssignmentChange> changes = AssignmentChange.Load(values, "AlbaBackupToS13/2003-03-03_235959/territories.txt");

            return changes;
        }

        public IList<AssignmentValues> LoadValues()
        {
            List<AssignmentValues> values = AssignmentValues
                .LoadFromCsv("AlbaBackupToS13/2003-03-03_235959/territories.txt");

            return values;
        }

        [Test]
        public void Load_Changes()
        {
            List<AssignmentChange> result = AssignmentChange.Load(LoadValues(), "AlbaBackupToS13/2003-03-03_235959/territories.txt");

            Assert.AreEqual(DateTime.Parse("2003-03-03 23:59:59"), result[0].TimeStamp);
            Assert.AreEqual("Bruce Wayne", result[0].Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), result[0].Date);
            Assert.AreEqual(AssignmentStatus.CheckedIn, result[0].Status);

            Assert.AreEqual(DateTime.Parse("2003-03-03 23:59:59"), result[1].TimeStamp);
            Assert.AreEqual("Clark Kent", result[1].Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), result[1].Date);
            Assert.AreEqual(AssignmentStatus.CheckedOut, result[1].Status);
        }

        [Test]
        public void BadPath_ReturnsEmpty()
        {
            var results = AssignmentChange.Load(
                LoadValues(), 
                path: "AlbaBackupToS13/2003-03-xx_235959/territories.txt");

            Assert.IsEmpty(results);

        }

        [Test]
        public void BadDate_ReturnsEmpty()
        {
            var results = AssignmentChange.Load(
                LoadValues(),
                path: "AlbaBackupToS13/2003-03-33_235959/territories.txt");

            Assert.IsEmpty(results);
        }

    }
}
