﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            //RemoveBlanks();
            SplitAtPoundSigns();
            ConvertPartsToResults();
        }

        void ReplaceWhiteSpaceWithSpace()
        {
            container.CompleteAddressToParse = NormalizeWhiteSpaceWithSpace(container.CompleteAddressToParse);
        }

        static string NormalizeWhiteSpaceWithSpace(string input)
        {
            return Regex.Replace(input, @"[^-0-9A-Z,#\/]", " ", RegexOptions.IgnoreCase);
        }

        void SplitAddressAtSpacesAndCommas()
        {
            var groups = container.CompleteAddressToParse.Split(new char[] { ',' }, System.StringSplitOptions.None).ToList();
            for(int g = 0; g < groups.Count; g++)
            {
                string group = groups[g];
                container.AddressGroups.Add(group);

                var newGroup = new List<string>();
                var parts = group
                    .Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                for(int p = 0; p < parts.Count; p++)
                {
                    string part = parts[p]; 
                    container.AddressParts.Add(part);
                    container.AddressPartsGroupIndex.Add(g);
                    container.AddressPartsPartIndex.Add(p);
                    newGroup.Add(part);
                }

                container.AddressPartsGrouped.Add(newGroup);
            }
        }

        //public void RemoveBlanks()
        //{
        //    var newList = new List<string>();
        //    foreach (var part in container.AddressParts)
        //    {
        //        if (!string.IsNullOrWhiteSpace(part))
        //        {
        //            newList.Add(part);
        //        }
        //    }

        //    container.AddressParts = newList;
        //}

        // Only public for tests
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

        // Only public for tests
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

        void ConvertPartsToResults()
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

        bool IsAlphaNumericHyphenAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\S)-(\S)$",
                RegexOptions.IgnoreCase);
        }

        bool IsPoundSignThenAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(#)(\S+)$",
                RegexOptions.IgnoreCase);
        }
    }
}
