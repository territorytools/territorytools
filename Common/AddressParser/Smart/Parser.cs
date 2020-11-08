using System;
using System.Text.RegularExpressions;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Parser
    {
        public ParsedAddress Parse(string text)
        {
            var parsed = new ParsedAddress();

            if(string.IsNullOrWhiteSpace(text))
            {
                return parsed;
            }

            var parts = text.Split(
                new char[] { ' ' }, 
                StringSplitOptions.RemoveEmptyEntries);

            string numberPattern = @"\d+";
            if(parts.Length > 0 && Regex.IsMatch(parts[0], numberPattern))
            {
                parsed.Street.Number = parts[0];
            }

            return parsed;
        }
    }
}
