using NUnit.Framework;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.Controllers.Tests.AlbaBackupToS13
{
    public class BackupFolderTests
    {
        [Test]
        public void FolderNotFound()
        {
            Assert.Throws<Exception>(() => BackupFolder.LoadFolder("Folder-Not-Found"));
        }

        [Test]
        public void CompareExpectedToAllFiles()
        {
            List<S13Entry> actuals = BackupFolder.LoadFolder("AlbaBackupToS13");
            List<S13Entry> expecteds = S13Entry.LoadCsv("AlbaBackupToS13\\expected.csv");

            Assert.AreEqual(expecteds.Count, actuals.Count);
            for (int i = 0; i < expecteds.Count; i++)
            {
                var expected = expecteds[i];
                var actual = actuals[i];
                Assert.AreEqual(expected.Number, actual.Number, $"Row {i}");
                Assert.AreEqual(expected.Publisher, actual.Publisher, $"Row {i}");
                Assert.AreEqual(expected.CheckedOut, actual.CheckedOut, $"Row {i}");
                Assert.AreEqual(expected.CheckedIn, actual.CheckedIn, $"Row {i}");
            }
        }
    }
}
