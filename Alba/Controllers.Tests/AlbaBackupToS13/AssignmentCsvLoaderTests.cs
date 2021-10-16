using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
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
            List<AssignmentChange> result = AssignmentCsvLoader
                .Load("AlbaBackupToS13/2004-04-04_125959/territories.txt");

            Assert.AreEqual("Bruce Wayne", result[0].Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), result[0].Date);
            Assert.AreEqual(AssignmentStatus.CheckedIn, result[0].Status);

            Assert.AreEqual("Clark Kent", result[1].Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), result[1].Date);
            Assert.AreEqual(AssignmentStatus.CheckedOut, result[1].Status);
        }

        [Test]
        public void GivenTwoValueEntriesSameNameInThenOut_ShouldBeOneS13Entry()
        {
            List<S13Entry> entries = AssignmentCsvLoader
                .LoadS13Entries("AlbaBackupToS13/1900-01-01_000000/territories.txt");

            var entry = entries[0];

            Assert.AreEqual("Bruce Wayne", entry.Publisher);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), entry.CheckOut);
            Assert.AreEqual(DateTime.Parse("2001-02-01"), entry.CheckIn);
        }

        [Test]
        public void GivenDifferentPublisherNameInOut_ShouldBeLastPublisher()
        {
            List<S13Entry> entries = AssignmentCsvLoader
                .LoadS13Entries("AlbaBackupToS13/1900-01-01_000000/territories.txt");

            var entry = entries[1];
      
            Assert.AreEqual("Superman", entry.Publisher);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), entry.CheckOut);
            Assert.AreEqual(DateTime.Parse("2002-03-02"), entry.CheckIn);

            var entry2 = entries[2];

            Assert.AreEqual("Clark Kent", entry2.Publisher);
            Assert.AreEqual(DateTime.Parse("2002-05-05"), entry2.CheckOut);
            Assert.AreEqual(null, entry2.CheckIn);
        }

        [Test]
        public void GivenInThenOut_ShouldAddTwo()
        {
            List<S13Entry> entries = AssignmentCsvLoader
                .LoadS13Entries("AlbaBackupToS13/1900-01-01_000000/territories.txt");

            var entry = entries[3];

            Assert.AreEqual("Lana Lang", entry.Publisher);
            Assert.AreEqual(null, entry.CheckOut);
            Assert.AreEqual(DateTime.Parse("2003-01-01"), entry.CheckIn);

            var entry2 = entries[4];

            Assert.AreEqual("Lana Lang", entry2.Publisher);
            Assert.AreEqual(DateTime.Parse("2003-02-02"), entry2.CheckOut);
            Assert.AreEqual(null, entry2.CheckIn);
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
