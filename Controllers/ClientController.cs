using AlbaClient.AlbaServer;
using AlbaClient.AzureMaps;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using AlbaClient.Nominatim;
using Controllers.AlbaServer;
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
                new ApplicationBasePath("https://", "www.alba-website-here.com", "/alba"));
        }

        public void UploadKmlButtonClick()
        {
            int delay = 1000;
            int.TryParse(view.UploadDelayMs, out delay);

            new UploadKmlFile(view, client, delay).Upload();
        }

        public void ImportAddressButtonClick(string path)
        {
            new ImportAddress(view, client, 0).Upload(path);
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

                new DownloadAddressExport(client).SaveAs(fileName);

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
