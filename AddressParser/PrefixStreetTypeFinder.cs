using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public abstract class PrefixStreetTypeFinder : Finder
    {
        public PrefixStreetTypeFinder(AddressParseContainer container)
            : base(container)
        {
            fullPrefixMatches = new List<AddressPartResult>();
            firstPrefixMatches = new List<AddressPartResult>();
            secondPrefixMatches = new List<AddressPartResult>();
        }


        protected abstract List<string> GetFirstPartPrefixes();

        protected abstract List<string> GetSecondPartPrefixes();

        protected abstract List<string> GetComletePrefixes();

        protected List<AddressPartResult> fullPrefixMatches;

        protected List<AddressPartResult> firstPrefixMatches;

        protected List<AddressPartResult> secondPrefixMatches;

        protected AddressPartResult fullPrefix;

        protected AddressPartResult firstPrefix;

        protected AddressPartResult secondPrefix;

        protected override bool PossibleMatchesWereFound()
        {
            return fullPrefixMatches.Count > 0 
                || firstPrefixMatches.Count > 0 
                || secondPrefixMatches.Count > 0;
        }

        protected override void FindMatch()
        {
            if (fullPrefixMatches.Count == 1)
            {
                FindFullPrefixMatch();
            }
            else if (firstPrefixMatches.Count > 0 && secondPrefixMatches.Count > 0)
            {
                FindTwoPartPrefixMatch();
            }
        }

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (MatchesFullPrefix(result.Value))
            {
                fullPrefixMatches.Add(result);
            }
            else if (MatchesFirstPartOfPrefix(result.Value))
            {
                firstPrefixMatches.Add(result);
            }
            else if (MatchesSecondPartOfPrefix(result.Value))
            {
                secondPrefixMatches.Add(result);
            }
        }


        private void FindFullPrefixMatch()
        {
            fullPrefix = fullPrefixMatches.Single();

            if (ANumberIsAfterPrefix(fullPrefix))
            {
                SetStreetTypeTo(fullPrefix);
            }
        }

        private void FindTwoPartPrefixMatch()
        {
            firstPrefix = firstPrefixMatches.First();
            secondPrefix = secondPrefixMatches.First();

            if (firstPrefix.IsBefore(secondPrefix))
            {
                FindANumberAfterSecondPrefix();
            }
        }

        private void FindANumberAfterSecondPrefix()
        {
            if (ANumberIsAfterPrefix(secondPrefix))
            {
                var combined = CombineFirstAndSecondResult(firstPrefix, secondPrefix);

                SetStreetTypeTo(combined);
            }
        }

        private AddressPartResult CombineFirstAndSecondResult(
            AddressPartResult first,
            AddressPartResult second)
        {
            var result = new AddressPartResult()
            {
                Value = first.Value + " " + second.Value,
                Index = second.Index,
            };            
            
            return result;
        }

        protected virtual void SetStreetTypeTo(AddressPartResult result)
        {
            parsedAddress.StreetType = result;
        }

        private bool ANumberIsAfterPrefix(AddressPartResult prefix)
        {
            
            if (prefix.IsLastItemIn(container.AddressPartResults))
            {
                return false;
            }

            var afterPrefix = prefix.GetItemAfterMeIn(container.AddressPartResults);

            return IsNumber(afterPrefix.Value);
        }

        private bool IsNumber(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d+)$",
                RegexOptions.IgnoreCase);
        }

        private bool MatchesFullPrefix(string value)
        {
            var streetTypes = GetComletePrefixes();

            return streetTypes
                .Exists(s => s.Equals(
                    value,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        private bool MatchesFirstPartOfPrefix(string value)
        {
            var streetTypes = GetFirstPartPrefixes();

            return streetTypes
                .Exists(s => s.Equals(
                    value,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        private bool MatchesSecondPartOfPrefix(string value)
        {
            var streetTypes = GetSecondPartPrefixes();

            return streetTypes
                .Exists(s => s.Equals(
                    value,
                    StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
