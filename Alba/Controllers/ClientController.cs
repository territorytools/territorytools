using AlbaClient.AlbaServer;
using AlbaClient.AzureMaps;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using Controllers.UseCases;
using System;

namespace AlbaClient
{
    public class ClientController
    {
        private IClientView view;

        public ClientController(IClientView view)
        {
            this.view = view;
        }

        public void UploadKmlButtonClick()
        {
            int delay = 1000;
            int.TryParse(view.UploadDelayMs, out delay);

            new UploadKmlFile(view, Client(), delay).Upload();
        }

        public void ImportAddressButtonClick(string path)
        {
            new ImportAddress(view, Client(), 0).Upload(path);
        }

        public void GeocodeAddressesClick(string path, string key)
        {
            try
            {
                GeocodeCsvAddressesFrom(path, key);
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);
            }
        }

        AuthorizationClient Client()
        {
            return new AuthorizationClient(
                new CookieWebClient(),
                new ApplicationBasePath("https://", view.AlbaHostText, "/alba"));
        }

        private void GeocodeCsvAddressesFrom(string path, string key)
        {
            var amWebClient = new CookieWebClient();
            var amBasePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "atlas.microsoft.com",
                applicationPath: "/");

            var amClient = new AzureMapsClient(
               webClient: amWebClient,
               basePath: amBasePath,
               subscriptionKey: key);

            int geocoded = 0;
            int alreadyGeocode = 0;
            var addresses = LoadCsvAddresses.LoadFrom(path);
            foreach (var address in addresses)
            {
                if (address.Latitude == null
                    || address.Longitude == null
                    || address.Latitude == 0
                    || address.Longitude == 0)
                {
                    var coordinates = new AzureMapsmGeocodeAddress(view, amClient)
                        .Geocode(address);

                    address.Latitude = coordinates.Latitude;
                    address.Longitude = coordinates.Longitude;

                    geocoded++;
                }
                else
                {
                    alreadyGeocode++;
                }
            }

            view.AppendResultText($"\nTotal Addresses: {(geocoded + alreadyGeocode)}");
            view.AppendResultText($"\nGeocoded: {geocoded}");
            view.AppendResultText($"\nAlready Geocoded (Skipped): {alreadyGeocode}");

            var newPath = view.GetKmlFileNameToSaveAs(path, "csv");
            if (string.IsNullOrWhiteSpace(newPath))
            {
                // Cancelled
                view.AppendResultText($"\nGeocoding not saved.");

                return; 
            }

            LoadCsvAddresses.SaveTo(addresses, newPath);

            view.AppendResultText($"\nGeocoding saved to {newPath}");

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

                var territories = new DownloadKmlFile(Client())
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

                new DownloadAddressExport(Client()).SaveAs(fileName);

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

                new DownloadTerritoryAssignments(Client()).SaveAs(fileName);

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

                new DownloadUsers(Client()).SaveAs(fileName);

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
                new LogUserOntoAlba(view, Client()).Logon();
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }
    }
}
