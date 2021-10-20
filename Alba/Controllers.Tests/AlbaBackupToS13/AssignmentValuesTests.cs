using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Controllers.Tests.AlbaBackupToS13
{
    public class AssignmentValuesTests
    {
        [Test]
        public void GivenNullPath_ThrowsException()
        {
            Assert.That(
                () => AssignmentValues.LoadFromCsv(null),
                Throws.InstanceOf(typeof(Exception)));
        }

        [Test]
        public void GivenNonExistantFile_ThrowsException()
        {
            Assert.That(
                () => AssignmentValues.LoadFromCsv("non-existant-file.txt"),
                Throws.TypeOf(typeof(FileNotFoundException)));
        }

        [Test]
        public void GivenNormalFile_ShouldLoadTwoRowsSuccessfully()
        {
            List<AssignmentValues> result = AssignmentValues
                .LoadFromCsv("AlbaBackupToS13/2003-03-03_235959/territories.txt");

            Assert.That(result != null);
            Assert.AreEqual(4, result.Count, "Result count");

            AssertValues(result[0],
                number: "1",
                signedOut: null,
                signedOutTo: "",
                lastCompleted: DateTime.Parse("2001-01-01"),
                lastCompletedBy: "Bruce Wayne",
                kind: "Regular",
                description: "ABC123",
                status: "Available");

            AssertValues(result[1],
                number: "2002",
                signedOut: DateTime.Parse("2002-02-02"),
                signedOutTo: "Clark Kent",
                lastCompleted: null,
                lastCompletedBy: string.Empty,
                kind: "Regular",
                description: "ABC123",
                status: "Signed-out");

            Assert.AreEqual("3003", result[2].Number);
        }

        void AssertValues(
            AssignmentValues actual,
            string number = null,
            DateTime? signedOut = null,
            string signedOutTo = null,
            DateTime? lastCompleted = null,
            string lastCompletedBy = null,
            string kind = null,
            string description = null,
            string status = null)
        {
            Assert.AreEqual(number, actual.Number);
            Assert.AreEqual(signedOutTo ?? string.Empty, actual.SignedOutTo);
            Assert.AreEqual(signedOut, actual.SignedOut);
            Assert.AreEqual(lastCompletedBy ?? string.Empty, actual.LastCompletedBy);
            Assert.AreEqual(lastCompleted, actual.LastCompleted);
            Assert.AreEqual(kind, actual.Kind);
            Assert.AreEqual(description, actual.Description);
            Assert.AreEqual(status, actual.Status);
        }
    }
}
