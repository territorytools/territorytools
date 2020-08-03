using TerritoryTools.Alba.Controllers.AlbaServer;
using Controllers.AlbaServer;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class ImportAddress
    {
        private AuthorizationClient client;
        private int msDelay;

        public ImportAddress(AuthorizationClient client, int msDelay)
        {
            this.client = client;
            this.msDelay = msDelay;
        }

        public void Upload(string path, string languageFilePath)
        {
            if (client.BasePath == null)
            {
                throw new UserException("You are not logged on to Alba.  Please Logon.");
            }

            if (string.IsNullOrWhiteSpace(path))
                return;

            var languages = LanguageDownloader.LoadLanguagesFrom(languageFilePath);
            
            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                var addresses = csv.GetRecords<AlbaAddressImport>();
                foreach (var address in addresses)
                {
                    Thread.Sleep(msDelay);

                    int languageId = languages
                        .First(l => string.Equals(
                            l.Name, 
                            address.Language, 
                            System.StringComparison.OrdinalIgnoreCase))
                        .Id;

                    int statusId = AddressStatusText.Status[address.Status];

                    var save = new AlbaAddressSave
                    {
                        Address_ID = address.Address_ID,
                        Territory_ID = address.Territory_ID,
                        LanguageId = languageId,
                        StatusId = statusId,
                        Name = address.Name,
                        Suite = address.Suite,
                        Address = address.Address,
                        City = address.City,
                        Province = address.Province,
                        Postal_code = address.Postal_code,
                        Country = address.Country,
                        Latitude = address.Latitude,
                        Longitude = address.Longitude,
                        Telephone = address.Telephone,
                        Notes = address.Notes,
                        Notes_private = address.Notes_private
                    };

                    var saveUrl = RelativeUrlBuilder.SaveAddress(save);
                    var resultString = client.DownloadString(saveUrl);

                    // TODO: Need to geocode
                }
            }
        }
    }
}
