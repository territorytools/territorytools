using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerritoryTools.Entities.AddressParsers
{
    public class PostalCodeFinder : Finder
    {
        public PostalCodeFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        private List<AddressPartResult> possibleMatches;

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAPossibleMatch(result))
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
            foreach(var postalCode in possibleMatches)
            {
                FindMatchIn(postalCode);
            }
        }

        private void FindMatchIn(AddressPartResult postalCode)
        {
            if (IsInCorrectLocation(postalCode))
            {
                FindAndMatchLength(postalCode);
            }
        }

        private void FindAndMatchLength(AddressPartResult postalCode)
        {
            if (IsNormalLength(postalCode))
            {
                container.ParsedAddress.PostalCode = postalCode;
            }
            else if (HasZipExt(postalCode))
            {
                SetPostalCodeAndExt(postalCode);
            }
            else
            {
                var message = "PostalCode wrong length: " + postalCode.Value;
                throw new AddressParsingException(message);
            }
        }

        private void SetPostalCodeAndExt(AddressPartResult postalCode)
        {
            var firstHalf = new AddressPartResult()
            {
                Value = postalCode.Value.Substring(0, 5),
                Index = postalCode.Index,
            };

            var secondHalf = new AddressPartResult()
            {
                Value = postalCode.Value.Substring(6, 4),
                Index = postalCode.Index,
            };

            container.ParsedAddress.PostalCode = firstHalf;
            container.ParsedAddress.PostalCodeExt = secondHalf;
        }

        private static bool IsNormalLength(AddressPartResult postalCode)
        {
            return postalCode.Value.Length == 5;
        }

        private static bool HasZipExt(AddressPartResult postalCode)
        {
            return postalCode.Value.Length == 10;
        }

        private bool IsInCorrectLocation(AddressPartResult part)
        {
            return part.IsAfter(parsedAddress.StreetType)
                && (parsedAddress.UnitType.IsNotSet()
                    || part.IsAtLeastNAfter(2, parsedAddress.UnitType));
        }

        private bool IsAPossibleMatch(AddressPartResult result)
        {
            return IsPostalCodeFormat(result.Value);
        }

        public bool IsPostalCodeFormat(string value)
        {
            return Regex.IsMatch(
                value,
                @"^((\d{5,})|(\d{5,})-(\d{4,}))$",
                RegexOptions.IgnoreCase);
        }
    }
}
