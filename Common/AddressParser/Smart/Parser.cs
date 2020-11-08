using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Parser
    {
        List<string> words;
        List<string> unParsed;

        public Address Parse(string text)
        {
            var address = new Address();
            if (string.IsNullOrWhiteSpace(text))
            {
                return address;
            }

            words = text
                .Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            unParsed = words;

            address.Street.Number = FindStreetNumber(text);
            address.Postal.Code = FindPostalCode(text);
            address.Region.Code = FindRegionCode(text);

            return address;
        }

        string FindStreetNumber(string text)
        {
            string streetNumberPattern = @"^\d+$";
            if (unParsed.Count > 0 && Regex.IsMatch(unParsed[0], streetNumberPattern))
            {
                string number = unParsed[0];
                unParsed.RemoveAt(0);
                return number;
            }

            return string.Empty;
        }

        string FindPostalCode(string text)
        {
            string postalCodePattern = @"^\d{5}$";
            string lastWord = unParsed[unParsed.Count - 1];
            if (unParsed.Count > 0 && Regex.IsMatch(lastWord, postalCodePattern))
            {
                unParsed.RemoveAt(unParsed.Count - 1);
                return lastWord;
            }

            return string.Empty;
        }

        string FindRegionCode(string text)
        {
            string regionCodePattern = @"^[a-zA-Z]{2}$";
            string lastWord = unParsed[words.Count - 1];
            if (unParsed.Count > 0 && Regex.IsMatch(lastWord, regionCodePattern))
            {
                unParsed.RemoveAt(unParsed.Count - 1);
                return lastWord;
            }

            return string.Empty;
        }
    }
}
