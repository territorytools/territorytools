using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace TerritoryTools.Entities.AddressParsers
{
    public class PostalCodeExtFinder : Finder
    {
        public PostalCodeExtFinder(AddressParseContainer container)
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

            var isFourDigits = IsFourDigits(match.Value);
            var afterPostCode = match.IsAfter(postalCode);

            if (isFourDigits && afterPostCode)
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
            container.ParsedAddress.PostalCodeExt = possibleMatches.Single();
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }

        public bool IsFourDigits(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(\d{4,})$",
                RegexOptions.IgnoreCase);
        }
    }
}
