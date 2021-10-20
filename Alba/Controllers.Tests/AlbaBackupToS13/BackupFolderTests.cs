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
            Assert.Throws<Exception>(() => BackupFolder.Load("Folder-Not-Found"));
        }

        [Test]
        public void ListFolderNames()
        {
            string[] folders = BackupFolder.Load("AlbaBackupToS13");

            Assert.Contains("AlbaBackupToS13\\1900-01-01_000000\\territories.txt", folders);
            Assert.Contains("AlbaBackupToS13\\2003-03-03_235959\\territories.txt", folders);
            Assert.Contains("AlbaBackupToS13\\2004-04-04_235959\\territories.txt", folders);
            Assert.Contains("AlbaBackupToS13\\2005-05-05_235959\\territories.txt", folders);
        }

        [Test]
        public void ReturnOneFolder()
        {
            string[] paths = {
                "AlbaBackupToS13\\1900-01-01_000000\\territories.txt",
                "AlbaBackupToS13\\2003-03-03_235959\\territories.txt",
                "AlbaBackupToS13\\2004-04-04_235959\\territories.txt",
                "AlbaBackupToS13\\2005-05-05_235959\\territories.txt"
            };

            List<S13Entry> actuals = BackupFolder.LoadStuff(paths);
            List<S13Entry> expecteds = S13Entry.LoadCsv("AlbaBackupToS13\\expected.csv");

            Assert.AreEqual(expecteds.Count, actuals.Count);
            for (int i = 0; i < expecteds.Count; i++)
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
