using AlbaClient.AlbaServer;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using Controllers.UseCases;
using System;

namespace AlbaClient
{
    public class ClientController
    {
        private IClientView view;
        private AuthorizationClient client;

        public ClientController(IClientView view)
        {
            this.view = view;

            client = new AuthorizationClient(
                new CookieWebClient(),
                new ApplicationBasePath("http://", "www.alba-website-here.com", "/alba"));
        }

        public void UploadKmlButtonClick()
        {
            int delay = 1000;
            int.TryParse(view.UploadDelayMs, out delay);

            new UploadKmlFile(view, client, delay).Upload();
        }

        public void LogonButtonClick()
        {
            LogOn();
        }

        // TODO: Add one kml per territory
        // TODO: Open Multiple  kml files feature (for uploading)
        // TODO: Upload CSV files too (for convenience)
        // TODO: Download Addresses inside kml
        // TODO: Upload addresses from kml too

        public void LoadTerritoriesButtonClick()
        {
            try
            {
                view.AppendResultText("Territory Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"TerritoryBorders.{timeStamp}", "kml");

                var territories = new DownloadKmlFile(client)
                    .SaveAs(fileName);

                territories.ForEach(t => view.AppendResultText(Environment.NewLine + t.ToString()));
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }

        public void DownloadAllAddressesButtonClick()
        {
            try
            {
                view.AppendResultText("Download All Addresses Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Addresses.{timeStamp}", "txt");

                new DownloadAddresses(client).SaveAs(fileName);

                view.AppendResultText($"Saved to: {fileName}");
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }

        public string DownloadTerritoriyAssignments()
        {
            try
            {
                view.AppendResultText("Download Territory Assignments Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Assignments.{timeStamp}", "csv");

                new DownloadTerritoryAssignments(client).SaveAs(fileName);

                view.AppendResultText($"Saved to: {fileName}");

                return fileName;
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);

                return null;
            }
        }

        public string DownloadUsers()
        {
            try
            {
                view.AppendResultText("Download Users Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Users.{timeStamp}", "csv");

                new DownloadUsers(client).SaveAs(fileName);

                view.AppendResultText($"Saved to: {fileName}");

                return fileName;
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);

                return null;
            }
        }

        public void credentialsBoxesEnterKeyPressed()
        {
            LogOn();
        }

        private void LogOn()
        {
            try
            {
                new LogUserOntoAlba(view, client).Logon();
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }
    }
}
