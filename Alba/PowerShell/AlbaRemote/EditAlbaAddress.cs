﻿using Controllers.AlbaServer;
using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsData.Edit,"AlbaAddress")]
    public class EditAlbaAddress : AlbaConnectedCmdlet
    {
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public AlbaAddressImport Address { get; set; }

        [Parameter]
        public int UploadDelayMs { get; set; } = 300;
        
        [Parameter]
        public SwitchParameter PrintUriOnly { get; set; }

        AddressImporter importer;

        protected override void BeginProcessing()
        {
            importer = new AddressImporter(
                Connection, 
                UploadDelayMs,
                languages: Languages);
        }

        protected override void ProcessRecord()
        {
            try
            {
                string result = importer.Update(Address, PrintUriOnly.IsPresent);
                
                WriteVerbose($"Result: {result}");
            }
            catch(Exception e)
            {
                WriteVerbose(Address.ToAddressString());
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
