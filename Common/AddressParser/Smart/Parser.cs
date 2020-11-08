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

            // Search from the beginning
            address.Street.Number = FindStreetNumber();

            // Search backwards from the end
            address.Postal.Code = FindPostalCode();
            address.Region.Code = FindRegionCode();

            return address;
        }

        string FindStreetNumber()
        {
            string streetNumberPattern = @"^\d+$";
            string firstWord = FirstWord();
            if (unParsed.Count > 0 && Regex.IsMatch(firstWord, streetNumberPattern))
            {
                RemoveFirstWord();
                return firstWord;
            }

            return string.Empty;
        }

        string FindPostalCode()
        {
            string postalCodePattern = @"^\d{5}$";
            string lastWord = LastUnParsedWord();
            if (unParsed.Count > 0 && Regex.IsMatch(lastWord, postalCodePattern))
            {
                RemoveLastWord();
                return lastWord;
            }

            return string.Empty;
        }

        string FindRegionCode()
        {
            string regionCodePattern = @"^[a-zA-Z]{2}$";
            string lastWord = LastUnParsedWord();
            if (unParsed.Count > 0 && Regex.IsMatch(lastWord, regionCodePattern))
            {
                RemoveLastWord();
                return lastWord;
            }

            return string.Empty;
        }

        string FirstWord()
        {
            return unParsed.First();
        }

        string LastUnParsedWord()
        {
            return unParsed.Last();
        }

        void RemoveFirstWord()
        {
            unParsed.RemoveAt(0);
        }

        void RemoveLastWord()
        {
            unParsed.RemoveAt(unParsed.Count - 1);
        }
    }
}
