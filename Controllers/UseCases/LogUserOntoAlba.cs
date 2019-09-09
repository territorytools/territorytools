using AlbaClient.AlbaServer;
using AlbaClient.Models;
using System;

namespace AlbaClient.Controllers.UseCases
{
    public class LogUserOntoAlba
    {
        public const string k1MagicString = "94dd9e08c129c785f7f256e82fbe0a30e6d1ae40";

        private IClientView view;
        private AuthorizationClient client;

        public LogUserOntoAlba(IClientView view, AuthorizationClient client)
        {
            this.view = view;
            this.client = client;
        }

        public void Logon()
        {
            var credentials = new Credentials(view.AccountBoxText, view.UserBoxText, view.PasswordBoxText, k1MagicString);

            client.Authorize(credentials);

            view.LoadTerritoriesButtonEnabled = true;
            view.UploadKmlFilesButtonEnabled = true;
            view.DownloadAllAddressesButtonEnabled = true;
            view.DownloadTerritoryAssignmentsButtonEnabled = true;
            view.DownloadUsersButtonEnabled = true;

            view.AppendResultText(Environment.NewLine + "Logged on successfully." + Environment.NewLine);
        }
    }

}
