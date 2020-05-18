using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressNumberFinder : Finder
    {
        public AddressNumberFinder(AddressParseContainer container) 
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        private List<AddressPartResult> possibleMatches;

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsNumber(result.Value) 
                || IsNumberThenLetter(result.Value) 
                || IsNumberHyphenSomething(result.Value))
            {
                possibleMatches.Add(result);
            }
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }

        protected override void FindMatch()
        {
            if (container.Address.IsNotPhysical)
            {
                FindNonPhysicalAddressNumber();
            }
            else
            {
                FindPhysicalAddressNumber();
            }
        }

        private void FindPhysicalAddressNumber()
        {
            var match = possibleMatches.First();
            if (IsNumberThenLetter(match.Value))
            {
                var split = SplitNumberAtLetter(match.Value);
                var numberHalf = new AddressPartResult()
                {
                    Value = split[0],
                    Index = match.Index,
                };

                parsedAddress.Number = numberHalf;

                var letterHalf = new AddressPartResult()
                {
                    Value = split[1],
                    Index = match.Index
                };

                parsedAddress.NumberFraction = letterHalf;
            }
            else if (IsNumberHyphenSomething(match.Value))
            {
                var split = match.Value.Split('-');
                if(split.Length != 2)
                {
                    throw new AddressParsingException("Cannot split the following at hyphen: " + match.Value);
                }

                var numberHalf = new AddressPartResult()
                {
                    Value = split[0],
                    Index = match.Index,
                };

                parsedAddress.Number = numberHalf;

                var letterHalf = new AddressPartResult()
                {
                    Value = split[1],
                    Index = match.Index
                };

                parsedAddress.NumberFraction = letterHalf;
            }
            else if(IsNumber(match.Value))
            {
                parsedAddress.Number = match;
            }
        }

        private void FindNonPhysicalAddressNumber()
        { 
            var nextAddressPart = parsedAddress.StreetType
                .GetItemAfterMeIn(container.AddressPartResults); 

            if (IsNumber(nextAddressPart.Value))
            {
                parsedAddress.Number = nextAddressPart;
            }
        }


        private bool IsNumber(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+)$",
                RegexOptions.IgnoreCase);
        }

        private bool IsNumberThenLetter(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+)([A-Z])$",
                RegexOptions.IgnoreCase);
        }

        private bool IsNumberHyphenSomething(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+)-(\S+)$",
                RegexOptions.IgnoreCase);
        }


        public static List<string> SplitNumberAtLetter(string value)
        {
            var pattern = @"^(\d+)([A-Z])$";
            var matches = new List<string>();

            var match = Regex.Match(value, pattern, RegexOptions.IgnoreCase);

            var number = match.Groups[1];
            var letter = match.Groups[2];

            if (number.Success && letter.Success)
            {
                matches.Add(number.Value);
                matches.Add(letter.Value);
            }
            else
            {
                throw new Exception("Cannot split number then letter");
            }

            return matches;
        }

    }
}
