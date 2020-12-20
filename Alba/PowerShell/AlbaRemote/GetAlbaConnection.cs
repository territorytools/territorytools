using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get , nameof(AlbaConnection))]
    [OutputType(typeof(AlbaConnection))]
    public class GetAlbaConnection : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string AlbaHost { get; set; }

        [Parameter(Mandatory = true)]
        public string Account { get; set; }

        [Parameter(Mandatory = true)]
        public PSCredential Credential { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(GetConnection());
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }

        public AlbaConnection GetConnection()
        {
            try
            {
                var client = new AlbaConnection(
                    new CookieWebClient(),
                    new ApplicationBasePath("https://", AlbaHost, "/alba"));

                IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(
                    Credential.Password);

                var creds = new Credentials(
                    account: Account, 
                    user: Credential.UserName, 
                    password: Marshal.PtrToStringUni(ptr));

                client.Authenticate(creds);

                // Persist this connection to an environment variable
                SessionState.PSVariable.Set(
                    name: Names.CurrentAlbaConnection.ToString(), 
                    value: client);

                return client;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
