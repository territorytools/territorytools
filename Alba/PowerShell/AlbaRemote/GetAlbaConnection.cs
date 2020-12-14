using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"AlbaConnection")]
    [OutputType(typeof(AlbaConnection))]
    public class GetAlbaConnection : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string AlbaHost { get; set; }

        [Parameter(Mandatory = true)]
        public string Account { get; set; }
        
        [Parameter(Mandatory = true)]
        public string User { get; set; }

        [Parameter(Mandatory = true)]
        public string Password { get; set; }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            try
            {
                WriteObject(GetClient());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AlbaConnection GetClient()
        {
            try
            {
                var client = new AlbaConnection(
                    new CookieWebClient(),
                    new ApplicationBasePath("https://", AlbaHost, "/alba"));

                var creds = new Credentials(Account, User, Password);

                client.Authenticate(creds);

                SessionState.PSVariable.Set("CurrentAlbaConnection", client);

                return client;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
