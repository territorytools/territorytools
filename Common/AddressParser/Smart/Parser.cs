using System;
using System.Text.RegularExpressions;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Parser
    {
        public Address Parse(string text)
        {
            var address = new Address();
            if (string.IsNullOrWhiteSpace(text))
            {
                return address;
            }

            address.Street.Number = FindStreetNumber(text);

            return address;
        }

        static string FindStreetNumber(string text)
        {
            var parts = text.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            string numberPattern = @"\d+";
            if (parts.Length > 0 && Regex.IsMatch(parts[0], numberPattern))
            {
                return parts[0];
            }

            return string.Empty;
        }
    }
}
