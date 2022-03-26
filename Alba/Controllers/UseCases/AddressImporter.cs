using Controllers.AlbaServer;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class AddressImporter
    {
        private AlbaConnection client;
        private int msDelay;
        List<AlbaLanguage> languages;

        public AddressImporter(
            AlbaConnection client, 
            int msDelay, 
            List<AlbaLanguage> languages = null,
            string languageFilePath = null)
        {
            this.languages = languages;

            if (string.IsNullOrWhiteSpace(languageFilePath)
                && (languages == null || languages.Count == 0))
            {
                throw new ArgumentNullException(nameof(languages));
            }

            if (languages == null)
            {
                this.languages = LanguageDownloader.LoadLanguagesFrom(languageFilePath);
            }

            this.client = client;
            this.msDelay = msDelay;
        }

        public string Update(AlbaAddressImport address, bool printUrlOnly = false)
        {
            if (client.BasePath == null)
            {
                throw new UserException("You are not logged on to Alba.  Please Logon.");
            }

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            AlbaAddressSave save = Convert(address);

            var url = RelativeUrlBuilder.UpdateAddress(save);
            
            if(printUrlOnly)
            {
                return url;
            }

            var resultString = client.DownloadString(url);

            return resultString;
        }

        public string Add(AlbaAddressImport address)
        {
            try 
            {
                if (client.BasePath == null)
                {
                    throw new UserException("You are not logged on to Alba.  Please Logon.");
                }

                if (address == null)
                {
                    throw new ArgumentNullException(nameof(address));
                }

                Thread.Sleep(msDelay);

                AlbaAddressSave save = Convert(address);

                var url = RelativeUrlBuilder.AddAddress(save);
                var resultString = client.DownloadString(url);

                return resultString;
            }
            catch(Exception e)
            {
                throw new Exception($"Error uploading address to Alba. Message: {e.Message}", e);
            }
        }

        public void AddFrom(string path)
        {
            if (client.BasePath == null)
            {
                throw new UserException("You are not logged on to Alba.  Please Logon.");
            }

            if (string.IsNullOrWhiteSpace(path))
                return;

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                var addresses = csv.GetRecords<AlbaAddressImport>();
                foreach (var address in addresses)
                {
                    Thread.Sleep(msDelay);

                    var save = Convert(address);
                    
                    // AddAddress for new addresses, SaveAddress for existing
                    var saveUrl = RelativeUrlBuilder.AddAddress(save);
                    var resultString = client.DownloadString(saveUrl);

                    // TODO: Need to geocode
                }
            }
        }

        AlbaAddressSave Convert(AlbaAddressImport address)
        {
            try
            {
                return TryConvert(address);
            }
            catch (Exception e)
            {
                throw new Exception($"Error converting address to AddressSave type.  Message: {e.Message}", e);
            }
        }

        private AlbaAddressSave TryConvert(AlbaAddressImport address)
        {
            if(address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if(languages == null || languages.Count == 0)
            {
                throw new Exception("There are no languages loaded. Please run Get-AlbaLangauge");
            }

            int languageId = languages
                .FirstOrDefault(l => string.Equals(
                    l?.Name,
                    address?.Language,
                    StringComparison.OrdinalIgnoreCase))
                ?.Id ?? 1; // 1 = Unknown

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

            return save;
        }
    }
}
