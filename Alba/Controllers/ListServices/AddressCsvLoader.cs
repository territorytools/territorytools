using Controllers.AlbaServer;
using System;
using System.Collections.Generic;
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
        public static PhoneNumberAddResults AddPhoneNumbers1And2(
            IEnumerable<AddressCsv> numbers,
            IEnumerable<AlbaAddressExport> addresses,
            int territoryId = 0)
        {
            var output = new List<AlbaAddressImport>();
            var map = addresses.ToDictionary(a => (int)a.Address_ID);

            var errors = new List<AddressCsv>();
            foreach (var number in numbers)
            {
                if (!map.ContainsKey((int)number.Address_ID))
                {
                    errors.Add(number);
                    continue;
                }

                var address = map[(int)number.Address_ID];

                address.Notes_private += AppendAsLine(
                    address.Notes_private,
                    $"List-Service-Added-Date: {DateTime.Now:yyyy-MM-dd}");

                string exsistingTelephone = string.IsNullOrWhiteSpace(address.Telephone)
                    ? "NONE"
                    : address.Telephone;

                string phone1 = string.IsNullOrWhiteSpace(number.Phone1)
                    ? "NONE"
                    : number.Phone1;

                string phone2 = string.IsNullOrWhiteSpace(number.Phone2)
                    ? "NONE"
                    : number.Phone2;

                string tel = string.IsNullOrWhiteSpace(address.Telephone)
                    ? "NONE"
                    : address.Telephone;

                address.Notes_private += AppendAsLine(
                    address.Notes_private,
                    $"List-Service-Existing-Telephone: {tel}");

                address.Notes_private += AppendAsLine(
                    address.Notes_private,
                    $"List-Service-Phone1: {phone1}");

                address.Notes_private += AppendAsLine(
                    address.Notes_private,
                    $"List-Service-Phone2: {phone2}");

                address.Notes_private += AppendAsLine(
                   address.Notes_private,
                   $"List-Service-Source-Territory-Number: {address.Territory_number}");

                // Delete duplicates 
                if (AreEqual(address.Telephone, number.Phone1))
                {
                    number.Phone1 = null;
                }

                // Delete duplicates
                if (AreEqual(address.Telephone, number.Phone2)
                    || AreEqual(number.Phone1, number.Phone2))
                {
                    number.Phone2 = null;
                }

                if (territoryId > 0
                    && (!string.IsNullOrWhiteSpace(number.Phone1)
                    || !string.IsNullOrWhiteSpace(number.Phone2)))
                {
                    address.Territory_ID = territoryId;
                }

                // Shift Phone2 to Phone1
                if (string.IsNullOrWhiteSpace(number.Phone1))
                {
                    number.Phone1 = number.Phone2;
                    number.Phone2 = null;
                }

                // Shift Phone1 to Telephone
                if (string.IsNullOrWhiteSpace(address.Telephone))
                {
                    address.Telephone = number.Phone1;
                    number.Phone1 = number.Phone2;
                    number.Phone2 = null;
                }

                if (!string.IsNullOrWhiteSpace(number.Phone1))
                {
                    address.Notes += AppendAsLine(
                        address.Notes,
                        $"Extra-Phone-1: { number.Phone1}");
                }

                if(!string.IsNullOrWhiteSpace(number.Phone2))
                {
                    address.Notes += AppendAsLine(
                        address.Notes,
                        $"Extra-Phone-2: { number.Phone2}");
                }

                var addressSaveData = AlbaAddressImport.From(address);

                output.Add(addressSaveData);
            }

            return new PhoneNumberAddResults
            {
                SuccessfulAddresses = output,
                Errors = errors
            };
        }

        static bool AreEqual(string first, string second)
        {
            return string.Equals(Normalize(first), Normalize(second));
        }

        static string Normalize(string phone)
        {
            if (phone == null)
            {
                return null;
            }

            return phone
                .Replace("\t", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty);
        }

        static string AppendAsLine(string target, string line)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return line;
            }
            else
            {
                return Environment.NewLine + line;
            }
        }
    }
}
