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

            List<AssignmentChange> changes = AssignmentChange.Load(values);

            return changes;
        }

        public IList<AssignmentValues> LoadValues()
        {
            List<AssignmentValues> values = AssignmentValues
                .LoadFromCsv("AlbaBackupToS13/2004-04-04_125959/territories.txt");

            return values;
        }

        // TODO THis one belongs in this file
        [Test]
        public void Load_Changes()
        {
            List<AssignmentChange> result = AssignmentChange.Load(LoadValues());

            Assert.AreEqual("Bruce Wayne", result[0].Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), result[0].Date);
            Assert.AreEqual(AssignmentStatus.CheckedIn, result[0].Status);

            Assert.AreEqual("Clark Kent", result[1].Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), result[1].Date);
            Assert.AreEqual(AssignmentStatus.CheckedOut, result[1].Status);
        }

        //public void AssertValues(AssignmentValues expected, AssignmentValues actual)
        //{
        //    Assert.AreEqual(expected.Number, actual.Number);
        //    Assert.AreEqual(expected.SignedOutTo, actual.SignedOutTo);
        //    Assert.AreEqual(expected.SignedOut, actual.SignedOut);
        //    Assert.AreEqual(expected.LastCompletedBy, actual.LastCompletedBy);
        //    Assert.AreEqual(expected.LastCompleted, actual.LastCompleted);
        //    Assert.AreEqual(expected.Kind, actual.Kind);
        //    Assert.AreEqual(expected.Description, actual.Description);
        //    Assert.AreEqual(expected.Status, actual.Status);
        //}

        //public void AssertValues(
        //    AssignmentValues actual,
        //    string number = null,
        //    DateTime? signedOut = null,
        //    string signedOutTo = null,
        //    DateTime? lastCompleted = null,
        //    string lastCompletedBy = null,
        //    string kind = null,
        //    string description = null,
        //    string status = null)
        //{
        //    Assert.AreEqual(number, actual.Number);
        //    Assert.AreEqual(signedOutTo ?? string.Empty, actual.SignedOutTo);
        //    Assert.AreEqual(signedOut, actual.SignedOut);
        //    Assert.AreEqual(lastCompletedBy ?? string.Empty, actual.LastCompletedBy);
        //    Assert.AreEqual(lastCompleted, actual.LastCompleted);
        //    Assert.AreEqual(kind, actual.Kind);
        //    Assert.AreEqual(description, actual.Description);
        //    Assert.AreEqual(status, actual.Status);
        //}
    }
}
