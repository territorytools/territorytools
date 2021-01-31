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
        [Parameter]
        public string AlbaHost { get; set; }

        [Parameter]
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
                AlbaHost = string.IsNullOrWhiteSpace(AlbaHost)
                    ? Environment.GetEnvironmentVariable("ALBA_HOST")
                    : AlbaHost;

                Account = !string.IsNullOrWhiteSpace(Account)
                    ? Environment.GetEnvironmentVariable("ALBA_ACCOUNT")
                    : Account;

                User = !string.IsNullOrWhiteSpace(User)
                    ? Environment.GetEnvironmentVariable("ALBA_USER")
                    : User;

                Password = !string.IsNullOrWhiteSpace(Password)
                    ? Environment.GetEnvironmentVariable("ALBA_PASSWORD")
                    : Password;

                if (string.IsNullOrWhiteSpace(AlbaHost)
                    || string.IsNullOrWhiteSpace(Account))
                {
                    throw new ArgumentException($"Missing {nameof(AlbaHost)} and {nameof(Account)}");
                }

                if(string.IsNullOrWhiteSpace(User)
                    || string.IsNullOrWhiteSpace(Password))
                {
                    User = Credential.UserName;

                    IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(
                        Credential.Password);
                    
                    Password = Marshal.PtrToStringUni(ptr);
                }

                if (string.IsNullOrWhiteSpace(User)
                   || string.IsNullOrWhiteSpace(Password))
                {
                    throw new ArgumentException($"Missing {nameof(User)} and {nameof(Password)}");
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
