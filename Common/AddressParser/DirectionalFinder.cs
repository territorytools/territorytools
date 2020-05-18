using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerritoryTools.Entities.AddressParsers
{
    public abstract class DirectionalFinder : Finder
    {
        public DirectionalFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        public static string ToDirectionalAbbreviation(string full)
        {
            if (string.IsNullOrWhiteSpace(full))
            {
                return string.Empty;
            }

            string text = full.ToUpper();

            return text
                .Replace("NORTH", "N")
                .Replace("SOUTH", "S")
                .Replace("EAST", "E")
                .Replace("WEST", "W");
        }

        protected List<AddressPartResult> possibleMatches;

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAPossibleMatch(result))
            {
                CapitalizeDirectionalAbbreviation(result);

                possibleMatches.Add(result);
            }
        }

        protected abstract bool IsAPossibleMatch(AddressPartResult result);

        private static void CapitalizeDirectionalAbbreviation(AddressPartResult result)
        {
            if (result.Value.Length == 2)
            {
                result.Value = result.Value.ToUpper();
            }
        }

        public bool MatchesDirection(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(([NWSE]{1,2}|(NORTH|WEST|SOUTH|EAST)(NORTH|WEST|SOUTH|EAST)?))?$",
                RegexOptions.IgnoreCase);
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }
    }
}
