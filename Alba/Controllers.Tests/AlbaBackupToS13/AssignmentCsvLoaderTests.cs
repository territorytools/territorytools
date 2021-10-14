using NUnit.Framework;
using System;
using System.IO;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Controllers.Tests.AlbaBackupToS13
{
    public class AssignmentCsvLoaderTests
    {
        [Test]
        public void LoadFromNullPath_ThrowsException()
        {
            Assert.That(
                () => AssignmentCsvLoader.LoadFromCsv(null),
                Throws.InstanceOf(typeof(Exception)));
        }

        [Test]
        public void LoadNonExistantFile_ThrowsException()
        {
            Assert.That(
                () => AssignmentCsvLoader.LoadFromCsv("non-existant-file.txt"),
                Throws.TypeOf(typeof(FileNotFoundException)));
        }

        [Test]
        public void LoadTestFileInFolder_OK()
        {
            var result = AssignmentCsvLoader.LoadFromCsv("AlbaBackupToS13/2004-04-04_125959/territories.txt");

            Assert.That(result != null);
            Assert.AreEqual(3, result.Count, "Result count");

            AssertValues(result[0],
                number: "1001",
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

        [Test]
        public void Load_Changes()
        {
            var result = AssignmentCsvLoader.Load("AlbaBackupToS13/territories.txt");

            Assert.AreEqual("Bruce Wayne", result[0].Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), result[0].Date);
            Assert.AreEqual(AssignmentStatus.CheckedIn, result[0].Status);

            Assert.AreEqual("Clark Kent", result[1].Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), result[1].Date);
            Assert.AreEqual(AssignmentStatus.CheckedOut, result[1].Status);
        }

        public void AssertValues(AssignmentValues expected, AssignmentValues actual)
        {
            Assert.AreEqual(expected.Number, actual.Number);
            Assert.AreEqual(expected.SignedOutTo, actual.SignedOutTo);
            Assert.AreEqual(expected.SignedOut, actual.SignedOut);
            Assert.AreEqual(expected.LastCompletedBy, actual.LastCompletedBy);
            Assert.AreEqual(expected.LastCompleted, actual.LastCompleted);
            Assert.AreEqual(expected.Kind, actual.Kind);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Status, actual.Status);
        }

        public void AssertValues(
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
