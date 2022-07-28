using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.Models;
using System;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class LogUserOntoAlba
    {
        private IClientView view;
        private AlbaConnection client;

        public LogUserOntoAlba(IClientView view, AlbaConnection client)
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
            view.DownloadUserInfoButtonEnabled = true;

            view.AppendResultText(Environment.NewLine + "Logged on successfully." + Environment.NewLine);
        }
    }

}
