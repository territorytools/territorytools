using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaBackupToS13;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.ConvertTo, "S13")]
    [OutputType(typeof(S13EntryCsvRow))]
    public class ConvertToS13 : PSCmdlet
    {
        [Parameter]
        public string FolderPath { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var s13entries = BackupFolder.LoadFolder(FolderPath);
                WriteVerbose($"Entries Loaded: {s13entries.Count}");
                foreach (var entry in s13entries)
                {
                    WriteObject(new S13EntryCsvRow(entry));
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }

            //base.EndProcessing();
        }
    }
}
