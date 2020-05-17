using AlbaClient.AlbaServer;
using AlbaClient.Models;
using System;

namespace AlbaClient.Controllers.UseCases
{
    public class LogUserOntoAlba
    {
        private IClientView view;
        private AuthorizationClient client;

        public LogUserOntoAlba(IClientView view, AuthorizationClient client)
        {
            this.view = view;
            this.client = client;
        }

        public void Logon()
        {
            var credentials = new Credentials(
                view.AccountBoxText, 
                view.UserBoxText, 
                view.PasswordBoxText);

            client.Authenticate(credentials);

            view.LoadTerritoriesButtonEnabled = true;
            view.UploadKmlFilesButtonEnabled = true;
            view.DownloadAllAddressesButtonEnabled = true;
            view.DownloadTerritoryAssignmentsButtonEnabled = true;
            view.DownloadUsersButtonEnabled = true;
            view.UploadAddressesButtonEnabled = true;

            view.AppendResultText(Environment.NewLine + "Logged on successfully." + Environment.NewLine);
        }
    }

}
