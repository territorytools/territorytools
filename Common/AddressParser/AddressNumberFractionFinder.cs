using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TerritoryTools.Entities.AddressParsers
{
    public class AddressNumberFractionFinder : Finder
    {
        public AddressNumberFractionFinder(AddressParseContainer container)
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
            SetInstanceVariablesFromContainer();

            var isFraction = IsFraction(match.Value);
            var afterNumber = match.IsAfter(number);
            var twoBeforeStreetType = match.IsAtLeastNBefore(2, streetType);

            if (isFraction && afterNumber && twoBeforeStreetType)
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
            foreach (var match in possibleMatches)
            {
                values.Add(match.Value);
            }

            var value = string.Join(" ", values);

            var combined = new AddressPartResult()
            {
                Value = value,
                Index = possibleMatches.Max(m => m.Index),
            };

            container.ParsedAddress.NumberFraction = combined;
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }

        public bool IsFraction(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+)\/(\d+)$",
                RegexOptions.IgnoreCase);
        }

        //public bool IsNumericOrAlphaNumeric(string value)
        //{
        //    return Regex.IsMatch(
        //        value,
        //        @"^((\d+[A-Z]*)|([A-Z]+\d+))$",
        //        RegexOptions.IgnoreCase);
        //}

        //public bool IsOneOrTwoLetters(string value)
        //{
        //    return Regex.IsMatch(
        //        value,
        //        @"^([A-Z]{1,2})$",
        //        RegexOptions.IgnoreCase);
        //}
    }
}
