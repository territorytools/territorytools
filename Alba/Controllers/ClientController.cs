using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.AzureMaps;
using TerritoryTools.Alba.Controllers.UseCases;
using TerritoryTools.Alba.Controllers.Models;
using Controllers.UseCases;
using System;
using System.Threading;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

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

                new AddressImporter(AuthenticatedClient(), int.Parse(view.UploadDelayMs), languageFilePath: languageFilePath)
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

        AlbaConnection Client()
        {
            return new AlbaConnection(
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
                    var connection = AuthenticatedClient();
                    var resultString = connection.DownloadString(
                    RelativeUrlBuilder.ExportAddresses(
                        accountId: connection.AccountId,
                        territoryId: 0,
                        searchText: ""));

                    string text = AddressExportParser.Parse(resultString);
                    File.WriteAllText(fileName, text);

                    //new DownloadAddressExport(AuthenticatedClient())
                    //    .SaveAs(fileName, id);

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

        private AlbaConnection AuthenticatedClient()
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

        public string DownloadUserInfo()
        {
            try
            {
                view.AppendResultText("Download User Info Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                string fileName = view.GetFileNameToSaveAs($"UserInfo.{timeStamp}", "csv");

                string url = RelativeUrlBuilder.GetUserManagementPage();
                var json = AuthenticatedClient().DownloadString(url);
                string html = AlbaJsonResultParser.ParseDataHtml(json, "users");
                var users = DownloadUserManagementData.GetUsers(html);


                using (var writer = new StreamWriter(fileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))

                {
                    csv.WriteRecords(users);
                }

                view.AppendResultText($"Saved to: {fileName}");

                return fileName;
            }
            catch (Exception e)
            {
                view.ShowMessageBox(e.Message);

                return null;
            }
        }

        public string UploadUserInfo()
        {
            try
            {
                view.AppendResultText("Upload User Info Result:" + Environment.NewLine + Environment.NewLine);

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HHmm");
                //string fileName = view.GetFileNameToSaveAs($"UserInfo.{timeStamp}", "csv");
                string fileName = view.OpenFileDialog("*.csv;*.txt", "Open UserInfo CSV File");
                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    //Delimiter = "\t",
                    BadDataFound = null,
                    //PrepareHeaderForMatch = args => args.Header.ToLower()
                };

                string combinedResult = string.Empty;

                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, configuration))
                {
                    var users = csv.GetRecords<AlbaHtmlUser>().ToList();
                    foreach (AlbaHtmlUser user in users)
                    {
                        UserRoles role = UserRoles.None;
                        switch(user.Role)
                        {
                            case "Search":
                                role = UserRoles.Search;
                                break;
                            case "User":
                                role = UserRoles.User;
                                break;
                            case "Assistant":
                                role = UserRoles.Assistant;
                                break;
                            case "Account owner":
                                role = UserRoles.AccountOwner;
                                break;
                        }

                        var request = new AddUserRequest
                        {
                            UserName = user.UserName,
                            //Password = // Probably not to upload
                            SendWelcomeEmail = false,
                            UserEmail = user.Email,
                            UserFullName = user.Name,
                            UserRole = role,
                            UserTelephone = user.Telephone,
                        };

                        string url = RelativeUrlBuilder.AddUser(request);
                        combinedResult += AuthenticatedClient().DownloadString(url);
                    }
                    
                }

                view.AppendResultText($"Saved to: {fileName}");
                view.AppendResultText($"Results: {combinedResult}");

                return combinedResult;
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
