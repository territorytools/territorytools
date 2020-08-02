using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.AzureMaps;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Alba.Controllers.Models;
using Controllers.UseCases;
using System;

namespace TerritoryTools.Alba.Controllers
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

            new UploadKmlFile(view, AuthenticatedClient(), delay)
                .Upload();
        }

        public void ImportAddressButtonClick(string path)
        {
            new ImportAddress(view, AuthenticatedClient(), 0)
                .Upload(path);
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

                var territories = new DownloadKmlFile(AuthenticatedClient())
                    .SaveAs(fileName);

                territories.ForEach(t => view.AppendResultText(Environment.NewLine + t.ToString()));
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }

        public void DownloadAllAddressesButtonClick(string accountId)
        {
            try
            {
                view.AppendResultText("Download All Addresses Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Addresses.{timeStamp}", "txt");

                if (int.TryParse(accountId, out int id))
                {
                    new DownloadAddressExport(AuthenticatedClient())
                        .SaveAs(fileName, id);

                    view.AppendResultText($"Saved to: {fileName}");
                }
                else
                {
                    view.ShowMessageBox($"Invalid account id {id}");
                }    
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }

        private AuthorizationClient AuthenticatedClient()
        {
            var client = Client();
            var creds = new Credentials(
                view.AccountBoxText,
                view.UserBoxText,
                view.PasswordBoxText);

            client.Authenticate(creds);
            return client;
        }

        public string DownloadTerritoriyAssignments()
        {
            try
            {
                view.AppendResultText("Download Territory Assignments Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Assignments.{timeStamp}", "csv");

                new DownloadTerritoryAssignments(AuthenticatedClient())
                    .SaveAs(fileName);

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

                new DownloadUsers(AuthenticatedClient())
                    .SaveAs(fileName);

                view.AppendResultText($"Saved to: {fileName}");

                return fileName;
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);

                return null;
            }
        }

        public string DownloadLanguages()
        {
            try
            {
                view.AppendResultText("Download Languages Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetKmlFileNameToSaveAs($"Languages.{timeStamp}", "html");

                new LanguageDownloader(AuthenticatedClient())
                    .SaveAs(fileName);

                view.AppendResultText($"Saved to: {fileName}");

                return fileName;
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);

                return null;
            }
        }

        public string LoadLanguages()
        {
            try
            {
                view.AppendResultText("Load Languages Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");

                string fileName = view.OpenKmlFileDialog("html");

                view.AppendResultText($"File Loaded: {fileName}");

                view.AppendResultText("Parsing language file...");

                var languages = LanguageDownloader.LoadLanguagesFrom(fileName);

                foreach (var language in languages)
                {
                    view.AppendResultText($"    {language.Id}: {language.Name}");
                }

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
                new LogUserOntoAlba(view, Client())
                    .Logon();
            }
            catch (Exception err)
            {
                view.ShowMessageBox(err.Message);
            }
        }
    }
}
