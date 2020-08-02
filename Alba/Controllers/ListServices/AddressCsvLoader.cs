using Controllers.AlbaServer;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TerritoryTools.Alba.ListServices
{
    public class PhoneNumberAddResults
    {
        public List<AlbaAddressImport> SuccessfulAddresses { get; set; }
        public List<AddressCsv> Errors { get; set; }
    }

    public class AddressCsvLoader
    {
        public static IEnumerable<AddressCsv> LoadFrom(string path)
        {
            var list = new List<AddressCsv>();
            if (string.IsNullOrWhiteSpace(path))
            {
                return list;
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                return csv.GetRecords<AddressCsv>().ToList();
            }
        }

        public static PhoneNumberAddResults AddPhoneNumbers1And2(
            IEnumerable<AddressCsv> numbers,
            IEnumerable<AlbaAddressExport> addresses)
        {
            var output = new List<AlbaAddressImport>();
            var map = addresses.ToDictionary(a => (int)a.Address_ID);

            var errors = new List<AddressCsv>();
            foreach(var number in numbers)
            {
                if(!map.ContainsKey((int)number.Address_ID))
                {
                    errors.Add(number);
                    continue;
                }

                var address = map[(int)number.Address_ID];
                address.Telephone = number.Phone1 ?? number.Phone2;

                if (!string.IsNullOrWhiteSpace(number.Phone1))
                {
                    address.Notes += Environment.NewLine +
                        $"List-Service-Phone1: {number.Phone1}";
                }

                if (!string.IsNullOrWhiteSpace(number.Phone2))
                {
                    address.Notes += Environment.NewLine +
                        $"List-Service-Phone2: {number.Phone2}";
                }

                address.Notes_private += Environment.NewLine +
                    $"{DateTime.Now:yyyy-MM-dd}: List Service phone numbers added.";

                var addressSaveData = AlbaAddressImport.From(address);

                output.Add(addressSaveData);
            }

            return new PhoneNumberAddResults
            {
                SuccessfulAddresses = output,
                Errors = errors
            };
        }
    }
}
