using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public class CityFinder : Finder
    {
        public CityFinder(AddressParseContainer container)
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

            var isAlphabetic = IsAlphabetic(match.Value);
            var afterStreetType = match.IsAfter(streetType);
            var afterDir = directionalSuffix.IsNotSet() || match.IsAfter(directionalSuffix);
            var afterUnitType = unitType.IsNotSet() || match.IsAtLeastNAfter(2, unitType);
            var beforeState = state.IsNotSet() || match.IsBefore(state);
            var beforePostalCode = postalCode.IsNotSet() || match.IsBefore(postalCode);

            if (isAlphabetic 
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
                Index = possibleMatches.Max(m => m.Index),
            };

            container.ParsedAddress.City = combined;
        }

        public bool IsAlphabetic(string value)
        {
            return Regex.IsMatch(
                value,
                @"^([A-Z]+)$",
                RegexOptions.IgnoreCase);
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }
    }
}
