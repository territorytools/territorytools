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

        [Parameter]
        public SwitchParameter Publishers { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                S13EntryCollection entries = BackupFolder.LoadFolder(FolderPath);
                WriteVerbose($"Entries Loaded: {entries.Count}");
                if (Publishers.IsPresent)
                {
                    foreach(string publisher in entries.Publishers)
                    {
                        WriteObject(publisher);
                    }
                }
                else
                {
                    foreach (var entry in entries)
                    {
                        WriteObject(new S13EntryCsvRow(entry));
                    }
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
