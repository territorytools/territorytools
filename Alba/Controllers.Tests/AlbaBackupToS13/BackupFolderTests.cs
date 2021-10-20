using NUnit.Framework;
using System;
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

            Assert.Contains("AlbaBackupToS13\\1900-01-01_000000", folders);
            Assert.Contains("AlbaBackupToS13\\2003-03-03_235959", folders);
            Assert.Contains("AlbaBackupToS13\\2004-04-04_235959", folders);
            Assert.Contains("AlbaBackupToS13\\2005-05-05_235959", folders);
        }
    }
}
