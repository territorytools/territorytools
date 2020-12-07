using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.AzureMaps;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Alba.Controllers.Models;
using Controllers.UseCases;
using System;
using System.Threading;

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

        public void ImportAddressButtonClick()
        {
            try
            {
                string languageFilePath = view.OpenFileDialog(
                    fileExt: "html", 
                    title: "Open Alba Language File");

                string addressFilePath = view.OpenFileDialog(
                    fileExt: "csv",
                    title: "Open Alba Address CSV File to Upload");

                if (string.IsNullOrWhiteSpace(languageFilePath) 
                    || string.IsNullOrWhiteSpace(addressFilePath))
                {
                    return;
                }

                new AddressImporter(AuthenticatedClient(), int.Parse(view.UploadDelayMs), languageFilePath)
                    .AddFrom(addressFilePath);
            }
            catch (UserException e)
            {
                view.ShowMessageBox(e.Message);
            }
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

        void GeocodeCsvAddressesFrom(string path, string key)
        {
            var amClient = AzureMapsmGeocodeAddress.AzureMapsClientFrom(key);

            int geocoded = 0;
            int alreadyGeocode = 0;
            int errors = 0;
            var addresses = LoadCsvAddresses.LoadFrom(path);
            foreach (var address in addresses)
            {
                try
                {
                    Thread.Sleep(40); // Free account limited to 50 per second, or every 20ms
                    if (address.Latitude == null
                        || address.Longitude == null
                        || address.Latitude == 0
                        || address.Longitude == 0)
                    {
                        var coordinates = new AzureMapsmGeocodeAddress(amClient)
                            .CoordinatesFrom(address);

                        address.Latitude = coordinates.Latitude;
                        address.Longitude = coordinates.Longitude;

                        geocoded++;
                    }
                    else
                    {
                        alreadyGeocode++;
                    }
                }
                catch(AzureMapsmGeocodeAddressException e)
                {
                    view.ShowMessageBox(e.Message);
                }
                catch(Exception)
                {
                    errors++;
                    address.Latitude = 0.0;
                    address.Longitude = 0.0;
                }
            }

            view.AppendResultText($"\nTotal Addresses: {(geocoded + alreadyGeocode)}");
            view.AppendResultText($"\nGeocoded: {geocoded}");
            view.AppendResultText($"\nAlready Geocoded (Skipped): {alreadyGeocode}");
            view.AppendResultText($"\nErrors: {errors}");

            var newPath = view.GetFileNameToSaveAs(path, "csv");
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
                string fileName = view.GetFileNameToSaveAs($"TerritoryBorders.{timeStamp}", "kml");

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
                string fileName = view.GetFileNameToSaveAs($"Addresses.{timeStamp}", "txt");

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
                string fileName = view.GetFileNameToSaveAs($"Assignments.{timeStamp}", "csv");

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
                string fileName = view.GetFileNameToSaveAs($"Users.{timeStamp}", "csv");

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
                string fileName = view.GetFileNameToSaveAs($"Languages.{timeStamp}", "html");

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

                string fileName = view.OpenFileDialog("html");

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
