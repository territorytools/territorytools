using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TerritoryTools.Entities.AddressParsers
{
    public class UnitNumberFinder : Finder
    {
        public UnitNumberFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        protected List<AddressPartResult> possibleMatches;

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAPossibleMatch(result))
            {
                CapitalizeUnitNumberLetters(result);

                possibleMatches.Add(result);
            }
        }

        private bool IsAPossibleMatch(AddressPartResult match)
        {
            SetInstanceVariablesFromContainer();

            //var isAlphabetic = IsAlphabetic(match.Value);
            var isAlphaNumeric = IsNumericOrAlphaNumeric(match.Value);
            var isOneOrTwoLetters = IsOneOrTwoLetters(match.Value);
            var isHyphenatedAlphaNumeric = IsHyphenatedAlphaNumeric(match.Value);
            var afterStreetType = match.IsAfter(streetType);
            var afterDir = directionalSuffix.IsNotSet() || match.IsAfter(directionalSuffix);
            var afterUnitType = unitType.IsNotSet() || match.IsAfter(unitType);
            var beforeState = state.IsNotSet() || match.IsBefore(state);
            var beforePostalCode = postalCode.IsNotSet() || match.IsBefore(postalCode);

            if ((isAlphaNumeric || isOneOrTwoLetters || isHyphenatedAlphaNumeric)
                && afterStreetType
                && afterDir
                && afterUnitType
                && beforeState
                && beforePostalCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void CapitalizeUnitNumberLetters(AddressPartResult result)
        {
            result.Value = result.Value.ToUpper();
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

            container.ParsedAddress.UnitNumber = combined;
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }


        public bool IsHyphenatedAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\S+-\S+)$",
                RegexOptions.IgnoreCase);
        }

        public bool IsNumericOrAlphaNumeric(string value)
        {
            return Regex.IsMatch(
                value,
                @"^((\d+[A-Z]*)|([A-Z]+\d+))$",
                RegexOptions.IgnoreCase);
        }

        public bool IsOneOrTwoLetters(string value)
        {
            return Regex.IsMatch(
                value,
                @"^([A-Z]{1,2})$",
                RegexOptions.IgnoreCase);
        }
    }
}
