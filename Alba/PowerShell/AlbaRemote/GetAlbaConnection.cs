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

        [Parameter]
        public string User { get; set; }

        [Parameter]
        public string Password { get; set; }

        [Parameter]
        public PSCredential Credential { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(User)
                    && string.IsNullOrWhiteSpace(Password)
                    && string.IsNullOrWhiteSpace(Credential.UserName)
                    && Credential.Password.Length == 0)
                {
                    throw new ArgumentException($"Missing {nameof(User)} and {nameof(Password)}, or {nameof(Credential)}");
                }

                if(string.IsNullOrWhiteSpace(User)
                    && string.IsNullOrWhiteSpace(Password))
                {
                    User = Credential.UserName;

                    IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(
                        Credential.Password);
                    
                    Password = Marshal.PtrToStringUni(ptr);
                }

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

                var creds = new Credentials(
                    account: Account, 
                    user: User, 
                    password: Password);

                client.Authenticate(creds);

                // Persist this connection to an session variable
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
