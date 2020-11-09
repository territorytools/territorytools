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
            if (string.IsNullOrWhiteSpace(address.Street.Number))
            {
                address.Street.Name.Name = FindNonStreet();
                if(!string.IsNullOrWhiteSpace(address.Street.Name.Name))
                {
                    address.Street.Number = FindNonStreetNumber();
                }
            }

            // Search backwards from the end
            address.Postal.Code = FindPostalCode();
            address.Region.Code = FindRegionCode();

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

        string FindNonStreet()
        {
            string pattern = @"^P(ost)?\.?\s*O(ffice)?\.?\s*B(\.|ox\b)?";
            string text = UnParsedText();
            //Regex.IsMatch(text, pattern)
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


            //string oneWordNonStreetPattern = @"^P\.?O\.?B(\.|ox)?$";
            //string firstWord = FirstWord();
            //if (unParsed.Count > 0 && Regex.IsMatch(firstWord, oneWordNonStreetPattern))
            //{
            //    RemoveFirstWord();
            //    return firstWord;
            //}

            //string nonStreetPattern1 = @"^(P(\.|ost)?(O\.?)?$";
            ////string nonPhysicalPattern2 = @"^(O(\.|ffice)?)$";
            //string nonStreetPattern3 = @"^B(\.|ox)?$";
            //if (unParsed.Count > 1 && Regex.IsMatch(firstWord, nonStreetPattern1))
            //{
            //    //RemoveFirstWord();
            //    string secondWord = unParsed[1]; //FirstWord();
            //    if(Regex.IsMatch(secondWord, nonStreetPattern3))
            //    {
            //        RemoveFirstWord();
            //        RemoveFirstWord();
            //        return firstWord;
            //    }
            //}

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
            if (unParsed.Count > 0)
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
            if (unParsed.Count > 0)
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
