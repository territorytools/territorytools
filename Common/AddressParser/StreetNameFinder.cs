using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerritoryTools.Entities.AddressParsers
{
    public class StreetNameFinder : Finder
    {
        public StreetNameFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        protected List<AddressPartResult> possibleMatches;
        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAPossibleMatch(result))
            {
                possibleMatches.Add(result);
            }
        }

        private bool IsAPossibleMatch(AddressPartResult match)
        {
            var streetType = parsedAddress.StreetType;
            var number = parsedAddress.Number;
            var fraction = parsedAddress.NumberFraction;
            var directional = parsedAddress.DirectionalPrefix;

            var alphabetic = IsAlphabetic(match.Value);
            var ordinal = IsNumberOrdinal(match.Value);
            var beforeStreetType = match.IsBefore(streetType);
            var afterNumber = match.IsAfter(number);
            var afterFraction = fraction.IsNotSet() || match.IsAfter(fraction);
            var afterDir = directional.IsNotSet() || match.IsAfter(directional);

            if ((alphabetic || ordinal) 
                && beforeStreetType 
                && afterNumber 
                && afterFraction 
                && afterDir)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void FindMatch()
        {
            var values = new List<string>();
            foreach(var match in possibleMatches)
            {
                values.Add(match.Value);
            }

            var value = string.Join(" ", values);

            var combined = new AddressPartResult()
            {
                Value = value,
                Index = parsedAddress.StreetType.Index - 1,
            };

            container.ParsedAddress.StreetName = combined;
        }

        public bool IsAlphabetic(string value)
        {
            return Regex.IsMatch(
                value,
                @"^([A-Z]+)$",
                RegexOptions.IgnoreCase);
        }

        public bool IsNumberOrdinal(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+(st|nd|rd|th))$",
                RegexOptions.IgnoreCase);
        }


        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }
    }
}
