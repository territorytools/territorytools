using System;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get,"Connection")]
    [OutputType(typeof(AuthorizationClient))]
    public class GetConnection : PSCmdlet
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

        public AuthorizationClient GetClient()
        {
            try
            {
                var client = new AuthorizationClient(
                    new CookieWebClient(),
                    new ApplicationBasePath("https://", AlbaHost, "/alba"));

                var creds = new Credentials(Account, User, Password);

                client.Authenticate(creds);

                return client;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
