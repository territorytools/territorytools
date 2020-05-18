using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressSplitter
    {
        public AddressSplitter (AddressParseContainer container)
	    {
            this.container = container;
	    }

        private AddressParseContainer container;

        public void SplitAndClean()
        {
            ReplaceWhiteSpaceWithSpace();
            SplitAddressAtSpacesAndCommas();
            RemoveBlanks();
            SplitAtPoundSigns();
            ConvertPartsToResults();
        }

        public void ReplaceWhiteSpaceWithSpace()
        {
            container.CompleteAddressToParse = NormalizeWhiteSpaceWithSpace(container.CompleteAddressToParse);
        }

        public static string NormalizeWhiteSpaceWithSpace(string input)
        {
            return Regex.Replace(input, @"[^-0-9A-Z,#\/]", " ", RegexOptions.IgnoreCase);
        }

        public void SplitAddressAtSpacesAndCommas()
        {
            var separators = new char[] { ' ', ','};

            container.AddressParts = container.CompleteAddressToParse
                .Split(separators)
                .ToList();
        }

        public void RemoveBlanks()
        {
            var newList = new List<string>();
            foreach (var part in container.AddressParts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    newList.Add(part);
                }
            }

            container.AddressParts = newList;
        }

        public void SplitAtPoundSigns()
        {
            var newList = new List<string>();
            foreach (var part in container.AddressParts)
            {
                if (IsPoundSignThenAlphaNumeric(part))
                {
                    newList.Add("#");
                    newList.Add(part.Substring(1));
                } 
                else if (!string.IsNullOrWhiteSpace(part))
                {
                    newList.Add(part);
                }
            }

            container.AddressParts = newList;
        }

        public void SplitAtHyphens()
        {
            var newList = new List<string>();
            foreach (var part in container.AddressParts)
            {
                if (part.Contains('-'))
                {
                    var splits = part.Split('-');
                    foreach (var split in splits)
                    {
                        if (!string.IsNullOrWhiteSpace(split))
                        {
                            newList.Add(split);
                        }
                    }
                }
                else
                {
                    newList.Add(part);
                }
            }

            container.AddressParts = newList;
        }

        private void ConvertPartsToResults()
        {
            var parts = container.AddressParts;
            var index = 0;
            foreach (var part in parts)
            {
                var result = new AddressPartResult()
                {
                    Value = part,
                    Index = index,
                };

                container.AddressPartResults.Add(result);

                index++;
            }
        }

        public bool IsAlphaNumericHyphenAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\S)-(\S)$",
                RegexOptions.IgnoreCase);
        }

        public bool IsPoundSignThenAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(#)(\S+)$",
                RegexOptions.IgnoreCase);
        }
    }
}
