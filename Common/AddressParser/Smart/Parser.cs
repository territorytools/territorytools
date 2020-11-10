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
        Address address = new Address();
        CityNameMatcher cityNameMatcher;

        public Parser(List<string> validCities)
        {
            cityNameMatcher = new CityNameMatcher(validCities);
        }

        public Address Parse(string text)
        {
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

            if (string.IsNullOrWhiteSpace(address.Street.Number))
            {
                address.Street.Name.Name = FindNonStreetName();
                if(!string.IsNullOrWhiteSpace(address.Street.Name.Name))
                {
                    address.Street.Number = FindNonStreetNumber();
                }
            }

            // Search backwards from the end
            address.Postal.Code = FindPostalCode();
            address.Region.Code = FindRegionCode();
            address.City.Name = FindCityName();

            if (!string.IsNullOrWhiteSpace(address.Street.Number)
                && string.IsNullOrWhiteSpace(address.Street.Name.Name))
            {
                string unit = FindUnit();
                var words = unit.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 0)
                {
                    address.Unit.Number = words.Last();
                }

                if(words.Length > 1)
                {
                    address.Unit.Type = words.First();
                }

                if(!string.IsNullOrWhiteSpace(address.Unit.Number)
                    && address.Unit.Number.StartsWith("#"))
                {
                    address.Unit.Number = address.Unit.Number.Substring(1);
                    if (string.IsNullOrWhiteSpace(address.Unit.Type))
                    {
                        address.Unit.Type = "#";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(address.Street.Number) 
                && string.IsNullOrWhiteSpace(address.Street.Name.Name))
            {
                address.Street.Name.Name = FindStreetName();
            }

            return address;
        }

        string FindStreetNumber()
        {
            string streetNumberPattern = @"^\d+$";
            string firstWord = FirstWord();
            if(unParsed.Count > 0 && Regex.IsMatch(firstWord, streetNumberPattern))
            {
                RemoveFirstWord();
                return firstWord;
            }

            return string.Empty;
        }

        string FindNonStreetName()
        {
            string pattern = @"^((P(ost)?\.?\s*O(ffice)?\.?\s*(B\.|B\b|Box\b))|Lot)";
            string text = UnParsedText();

            var r = new Regex(pattern, RegexOptions.IgnoreCase);
            var m = r.Match(text);
            
            if (m.Success)
            {
                int wordCount = m.Value.Split(' ').Length;
                for(int i = 0; i < wordCount; i++)
                {
                    RemoveFirstWord();
                }

                return m.Value;
            }

            return string.Empty;
        }

        string FindNonStreetNumber()
        {
            string nonStreetNumberPattern = @"^\d+$";
            string firstWord = FirstWord();
            if (unParsed.Count > 0 && Regex.IsMatch(firstWord, nonStreetNumberPattern))
            {
                RemoveFirstWord();
                return firstWord;
            }

            return string.Empty;
        }

        string FindPostalCode()
        {
            string postalCodePattern = @"^\d{5}$";
            // Postal Code should have at least three words before it and 
            // street number should already be parsed
            if (unParsed.Count >= 3 && !string.IsNullOrWhiteSpace(address.Street.Number))
            {
                string lastWord = LastWord();
                if (Regex.IsMatch(lastWord, postalCodePattern))
                {
                    RemoveLastWord();
                    return lastWord;
                }
            }

            return string.Empty;
        }

        string FindRegionCode()
        {
            string regionCodePattern = @"^[a-zA-Z]{2}$";
            // Region Code (Province/State) should have at least three words 
            // before it and street number should already be parsed
            if (unParsed.Count >= 3 && !string.IsNullOrWhiteSpace(address.Street.Number))
            {
                string lastWord = LastWord();
                if (Regex.IsMatch(lastWord, regionCodePattern))
                {
                    RemoveLastWord();
                    return lastWord;
                }
            }

            return string.Empty;
        }

        string FindCityName()
        {
            // City Name should have at least one word, the street name, before it
            if (unParsed.Count > 1 && !string.IsNullOrWhiteSpace(address.Region.Code))
            {
                var matched = cityNameMatcher.FindCityName(unParsed);
                if(matched.Length > 0)
                {
                    for(int i = 0; i < matched.Length; i++)
                    {
                        RemoveLastWord();
                    }

                    return string.Join(" ", matched);
                }
            }

            return string.Empty;
        }

        string FindUnit()
        {
            // Unit Type should have at least one word, the street name, before it
            string unitPattern = @"(#|Apartment|Apt|Suite|Ste|Unit|Cabin)\.?\s*#?\s*[0-9a-zA-Z][0-9a-zA-Z-]*$";
            if (unParsed.Count >= 1 && !string.IsNullOrWhiteSpace(address.Street.Number))
            {
                string text = UnParsedText();
                var r = new Regex(unitPattern, RegexOptions.IgnoreCase);
                var m = r.Match(text);
                if (m.Success)
                {
                    int wordCount = m.Value.Split(' ').Length;
                    for (int i = 0; i < wordCount; i++)
                    {
                        RemoveFirstWord();
                    }

                    return m.Value;
                }
            }

            return string.Empty;
        }

        string FindStreetName()
        {
            string pattern = @"^[0-9a-zA-Z][0-9a-zA-Z- ]*";
            string text = UnParsedText();

            var r = new Regex(pattern, RegexOptions.IgnoreCase);
            var m = r.Match(text);

            if (m.Success)
            {
                int wordCount = m.Value.Split(' ').Length;
                for (int i = 0; i < wordCount; i++)
                {
                    RemoveFirstWord();
                }

                return m.Value;
            }

            return string.Empty;
        }

        string FirstWord()
        {
            return unParsed.First();
        }

        string LastWord()
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

        string UnParsedText()
        {
            return string.Join(" ", unParsed);
        }
    }
}
