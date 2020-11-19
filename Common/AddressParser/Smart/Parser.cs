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
        List<string> streetTypes;
        Dictionary<string, string> mapStreetTypes;
        List<string> prefixStreetTypes;

        public Parser(
            List<string> validCities,
            List<string> streetTypes,
            Dictionary<string, string> mapStreetTypes,
            List<string> prefixStreetTypes)
        {
            this.streetTypes = streetTypes;
            this.mapStreetTypes = mapStreetTypes;
            this.prefixStreetTypes = prefixStreetTypes;
            cityNameMatcher = new CityNameMatcher(validCities);
        }

        public bool Normalize { get; set; }

        public Address Parse(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return new Address();
            }

            text = text
                .Replace(',', ' ')
                .Replace('.', ' ')
                .Trim();

            if(text.Contains(","))
            {
                var columns = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if(columns.Length < 3)
                {
                    throw new Exception(
                        $"Not enough commas, you must have at least three, but no more than five. Address: {text}");
                }
                else if(columns.Length == 4)
                {
                    return Parse(
                        text: null, 
                        street: columns[0], 
                        unit: null, 
                        city: columns[1], 
                        region: columns[2], 
                        postal: columns[3]);
                } 
                else if(columns.Length == 5)
                {
                    return Parse(
                        text: null,
                        street: columns[0],
                        unit: columns[1],
                        city: columns[2],
                        region: columns[3],
                        postal: columns[4]);
                }
                else if (columns.Length > 5)
                {
                    throw new Exception(
                        $"Too many commas, you must have at least three, but no more than five. Address: {text}");
                }
            }

            return Parse(text, null, null, null, null, null);
        }
        
        public Address Parse(
            string text, 
            string street, 
            string unit, 
            string city, 
            string region, 
            string postal)
        {
            address = new Address();

            if (string.IsNullOrWhiteSpace(text))
            {
                return address;
            }

            if(Normalize)
            {
                text = text.ToUpper();
            }

            words = text
                .Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            unParsed = words;

            // Search from the beginning
            (address.Street.Number, address.Street.NumberFraction) = FindStreetNumber();

            if (string.IsNullOrWhiteSpace(address.Street.Number))
            {
                address.Street.Name.NamePrefix = FindNonStreetName();
                if(!string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix))
                {
                    address.Street.Number = FindNonStreetNumber();
                }
            }

            // Search backwards from the end
            address.Postal.Code = postal ?? FindPostalCode();
            address.Region.Code = region ?? FindRegionCode();
            if (!string.IsNullOrWhiteSpace(address.Region.Code))
            {
                address.City.Name = city ?? FindCityName();
            }

            // Find unit type and number
            if (!string.IsNullOrWhiteSpace(address.Street.Number)
                && string.IsNullOrWhiteSpace(address.Street.Name.Name))
            {
                if (unParsed.Count > 1)
                {
                    string u = FindUnit();
                    // TODO: If it's empty stop trying
                    var words = u.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 0)
                    {
                        address.Unit.Number = words.Last();
                    }

                    if (words.Length > 1)
                    {
                        address.Unit.Type = words.First();
                    }

                    if (!string.IsNullOrWhiteSpace(address.Unit.Number)
                        && address.Unit.Number.StartsWith("#"))
                    {
                        address.Unit.Number = address.Unit.Number.Substring(1);
                        if (string.IsNullOrWhiteSpace(address.Unit.Type))
                        {
                            address.Unit.Type = "#";
                        }
                    }
                }
            }

            // What remains is the street name
            if (!string.IsNullOrWhiteSpace(address.Street.Number) 
                && string.IsNullOrWhiteSpace(address.Street.Name.Name)
                && string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix))
            {
                address.Street.Name.DirectionalSuffix = FindDirectionalSuffix();
                address.Street.Name.DirectionalPrefix = string.IsNullOrWhiteSpace(address.Street.Name.DirectionalSuffix)
                    ? FindDirectionalPrefix()
                    : string.Empty;

                address.Street.Name.StreetTypePrefix = FindPrefixStreetType();
                address.Street.Name.StreetType = string.IsNullOrWhiteSpace(address.Street.Name.StreetTypePrefix)
                    ? FindStreetType()
                    : string.Empty;

                address.Street.Name.Name = FindStreetName();
            }

            // Checked required
            if(string.IsNullOrWhiteSpace(address.Street.Number)
                || (string.IsNullOrWhiteSpace(address.Street.Name.Name)
                  && string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix))
                || string.IsNullOrWhiteSpace(address.City.Name)
                || string.IsNullOrWhiteSpace(address.Region.Code))
            {
                address.FailedAddress = text;
            }

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(address.Street.Number))
            {
                errors.Add("Street.Number");
            }

            if (string.IsNullOrWhiteSpace(address.Street.Name.Name) 
                && string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix))
            {
                errors.Add("Street.Name.Name");
                errors.Add("Street.Name.NamePrefix");
            }

            if (string.IsNullOrWhiteSpace(address.City.Name))
            {
                errors.Add("City.Name");
            }

            if (string.IsNullOrWhiteSpace(address.Region.Code))
            {
                errors.Add("Region.Code");
            }

            address.ErrorMessage = string.Join(", ", errors);

            return address;
        }

        (string, string) FindStreetNumber()
        {
            string streetNumberPattern = @"^(\d+)-?([A-Z]|\d/\d)?$";
            string firstWord = FirstWord();
            var match = Regex.Match(firstWord, streetNumberPattern);
            if (unParsed.Count > 0 && match.Success)
            {
                RemoveFirstWord();
                
                return (
                    match.Groups[1].Value, 
                    match.Groups.Count > 2 ? match.Groups[2].Value : string.Empty);
            }

            return (string.Empty, string.Empty);
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
            string postalCodePattern = @"^\d{5}(-\d{4})?$";
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
            if ((string.IsNullOrWhiteSpace(address.Street.Name.Name) && unParsed.Count >= 3
                || !string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix) && unParsed.Count >= 2)
                && !string.IsNullOrWhiteSpace(address.Street.Number))
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
            // ...unless it's a PO Box in 'Street.Name.NamePrefix'
            if (unParsed.Count > 1 
                || (!string.IsNullOrWhiteSpace(address.Street.Name.NamePrefix) 
                    && !string.IsNullOrWhiteSpace(address.Street.Number)
                    && unParsed.Count >= 1))
            {
                var matched = cityNameMatcher.FindCityName(unParsed);
                if(matched.Length > 0)
                {
                    for(int i = 0; i < matched.Length; i++)
                    {
                        RemoveLastWord();
                    }

                    if (Normalize)
                    {
                        return string.Join(" ", matched).ToUpper();
                    }
                    else
                    {
                        return string.Join(" ", matched);
                    }
                }
                else
                {
                    string lastWord = LastWord();
                    RemoveLastWord();
                    return lastWord;
                }
            }

            return string.Empty;
        }

        string FindUnit()
        {
            // Unit Type should have at least one word, the street name, before it
            // TODO: Inject a list of unit types into this unitPattern
            string unitTypePattern = @"(#|Apartment|Apt|Suite|Ste|Unit|Cabin)\.?\s*#?\s*[0-9a-zA-Z][0-9a-zA-Z-]*$";
            string justTheNumberPattern = @"(\d+[0-9a-zA-Z]*|[a-zA-Z]+\d+|[0-9a-zA-Z]+-[0-9a-zA-Z]+)$";
            string justLettersPattern = @"([a-zA-Z]+)$";
            if (!string.IsNullOrWhiteSpace(address.Street.Number))
            {
                string text = UnParsedText();
                var unitTypeRegex = new Regex(unitTypePattern, RegexOptions.IgnoreCase);
                var unitTypeMatch = unitTypeRegex.Match(text);

                string lastWord = LastWord();
                var justTheNumberRegex = new Regex(justTheNumberPattern, RegexOptions.IgnoreCase);
                var justTheNumberMatch = justTheNumberRegex.Match(lastWord);

                var justLettersRegex = new Regex(justLettersPattern, RegexOptions.IgnoreCase);
                var justLettersMatch = justLettersRegex.Match(lastWord);

                if (unitTypeMatch.Success)
                {
                    int wordCount = unitTypeMatch.Value.Split(' ').Length;
                    for (int i = 0; i < wordCount; i++)
                    {
                        RemoveLastWord();
                    }

                    return unitTypeMatch.Value;
                }
                else if (justTheNumberMatch.Success)
                {
                    RemoveLastWord();

                    return justTheNumberMatch.Value;
                }
                else if (justLettersMatch.Success)
                {
                    if (unParsed.Count >= 3)
                    {
                        string wordBeforeLast = unParsed[unParsed.Count - 2];
                        string twoWordsBeforeLast = unParsed[unParsed.Count - 3];
                        if(IsDirectional(wordBeforeLast) && IsStreetType(twoWordsBeforeLast))
                        {
                            RemoveLastWord();
                            return lastWord;
                        }

                        if(!IsDirectional(lastWord)
                            && !IsStreetType(lastWord)
                            && IsStreetType(wordBeforeLast))
                        {
                            RemoveLastWord();
                            return lastWord;
                        }

                    }
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
                var words = m.Value.Split(' ');
                int wordCount = words.Length;

                for (int i = 0; i < wordCount; i++)
                {
                    RemoveFirstWord();
                }

                return m.Value;
            }

            return string.Empty;
        }

        string FindDirectionalPrefix()
        {
            if(unParsed.Count == 0)
            {
                return string.Empty;
            }

            string word = FirstWord();

            // If the second word is a street type, then treat this directional 
            // word as a name
            // Example: North Rd
            if (unParsed.Count == 2 && IsStreetType(unParsed[1]))
            {
                return string.Empty;
            }

            if(unParsed.Count > 1 && IsDirectional(word))
            {
                RemoveFirstWord();

                if (Normalize)
                {
                    return NormalizeDirectional(word);
                }
                else
                {
                    return word;
                }
            }

            return string.Empty;
        }

        bool IsStreetType(string word)
        {
            return streetTypes.Contains(word.ToUpper());
        }
        
        string FindDirectionalSuffix()
        {
            if(unParsed.Count > 1)
            {
                string word = LastWord();
                if (IsDirectional(word))
                {
                    RemoveLastWord();

                    if (Normalize)
                    {
                        return NormalizeDirectional(word);
                    }
                    else
                    {
                        return word;
                    }
                }
            }

            return string.Empty;
        }

        string NormalizeDirectional(string text)
        {
            return text
                .ToUpper()
                .Replace("NORTH", "N")
                .Replace("SOUTH", "S")
                .Replace("EAST", "E")
                .Replace("WEST", "W");
        }

        bool IsDirectional(string word)
        {
            string pattern = @"^(N|S|E|W|North|South|East|West)(E|W|East|West)?$";
            return Regex.IsMatch(word, pattern, RegexOptions.IgnoreCase);
        }

        string FindPrefixStreetType()
        {
            if(unParsed.Count == 0)
            {
                return string.Empty;
            }

            string word = FirstWord();
            if (prefixStreetTypes.Contains(word.ToUpper()))
            {
                if (unParsed.Count > 1)
                {
                    RemoveFirstWord();
                    return word;
                } 
                else if(unParsed.Count == 1 
                    && string.IsNullOrWhiteSpace(address.Unit.Type)
                    && !string.IsNullOrWhiteSpace(address.Unit.Number))
                {
                    RemoveFirstWord();
                    unParsed.Add(address.Unit.Number);
                    address.Unit.Number = string.Empty;
                    return word;
                }
            }

            return string.Empty;
        }

        string FindStreetType()
        {
            if(unParsed.Count == 0)
            {
                return string.Empty;
            }

            string word = LastWord();
            if (unParsed.Count > 1 && IsStreetType(word))
            {
                RemoveLastWord();

                if(Normalize)
                {
                    word = word.ToUpper();
                    if(mapStreetTypes.ContainsKey(word))
                    {
                        return mapStreetTypes[word];
                    }
                }

                return word;
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
