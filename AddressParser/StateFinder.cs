using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public class StateFinder : Finder
    {
        public StateFinder(AddressParseContainer container)
            : base(container)
        {
            possibleMatches = new List<AddressPartResult>();
        }

        private List<AddressPartResult> possibleMatches;

        protected override void FindPossibleMatch(AddressPartResult result)
        {
            if (IsAPossibleMatch(result))
            {
                CapitalizeStateAbbreviation(result);
                possibleMatches.Add(result);
            }
        }

        protected override bool PossibleMatchesWereFound()
        {
            return possibleMatches.Count > 0;
        }

        private static void CapitalizeStateAbbreviation(AddressPartResult result)
        {
            result.Value = result.Value.ToUpper();
        }

        protected override void FindMatch()
        {
            foreach (var match in possibleMatches)
            {
                if (IsTwoAfterStreetTypeAndUnitType(match))
                {
                    container.ParsedAddress.State = match;
                    break;
                }
            }
        }

        private bool IsTwoAfterStreetTypeAndUnitType(AddressPartResult part)
        {
            // A state is never immediately after the street type or unit type
            return IsTwoAfterStreetType(part) 
                && (parsedAddress.UnitType.IsNotSet()
                    || part.IsAtLeastNAfter(1, parsedAddress.UnitType));
        }

        private bool IsTwoAfterStreetType(AddressPartResult part)
        {
            return part.IsAtLeastNAfter(2, parsedAddress.StreetType);
        }

        private bool IsAPossibleMatch(AddressPartResult result)
        {
            return MatchesState(result.Value);
        }

        public bool MatchesState(string value)
        {
            return Regex.IsMatch(
                value,
                @"^((A[LKSZR])|(C[AOT])|(D[EC])|(F[ML])|(G[AU])|(HI)|(I[DLNA])|(K[SY])|(LA)|(M[EHDAINSOT])|(N[VHJMYCD])|(MP)|(O[HKR])|(P[WAR])|(RI)|(S[CD])|(T[NX])|(UT)|(V[TIA])|(W[AVIY]))$",
                RegexOptions.IgnoreCase);
        }
    }
}
