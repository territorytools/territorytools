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
        public void LoadTestFile_OK()
        {
            var result = AssignmentCsvLoader.LoadFromCsv("AlbaBackupToS13/territories.txt");

            Assert.That(result != null);
            Assert.AreEqual(3, result.Count, "Result count");

            Assert.AreEqual("1001", result[0].Number);
            Assert.AreEqual("", result[0].SignedOutTo);
            Assert.IsNull(result[0].SignedOut);
            Assert.AreEqual("Bruce Wayne", result[0].LastCompletedBy);
            Assert.AreEqual(DateTime.Parse("2001-01-01"), result[0].LastCompleted);
            Assert.AreEqual("Regular", result[0].Kind);
            Assert.AreEqual("ABC123", result[0].Description);
            Assert.AreEqual("Available", result[0].Status);

            Assert.AreEqual("2002", result[1].Number);
            Assert.AreEqual("Clark Kent", result[1].SignedOutTo);
            Assert.AreEqual(DateTime.Parse("2002-02-02"), result[1].SignedOut);
            Assert.AreEqual("", result[1].LastCompletedBy);
            Assert.IsNull(result[1].LastCompleted);
            Assert.AreEqual("Regular", result[1].Kind);
            Assert.AreEqual("ABC123", result[1].Description);
            Assert.AreEqual("Signed-out", result[1].Status);

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
    }
}
