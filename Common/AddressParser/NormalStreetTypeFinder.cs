using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Entities.AddressParsers
{
    public class NormalStreetTypeFinder : Finder
    {
        public NormalStreetTypeFinder(
            AddressParseContainer container, 
            IEnumerable<StreetType> streetTypes)
            : base(container)
        {
            possibleFullStreetTypes = new List<AddressPartResult>();
            possibleAbbreviatedStreetTypes = new List<AddressPartResult>();
            allPossibleStreetTypes = new List<AddressPartResult>();
            this.streetTypes = streetTypes.ToList();
        }

        private List<AddressPartResult> possibleFullStreetTypes;
        private List<AddressPartResult> possibleAbbreviatedStreetTypes;
        private List<AddressPartResult> allPossibleStreetTypes;
        private int abbreviatedCount;
        private int fullsCount;
        private int allCount;
        private List<StreetType> streetTypes;

        protected override bool PossibleMatchesWereFound()
        {
            return possibleFullStreetTypes.Count > 0
                || possibleAbbreviatedStreetTypes.Count > 0
                || allPossibleStreetTypes.Count > 0;
        }


        protected override void FindMatch()
        {
            CountPossibleStreetTypes();

            if (allCount == 1) //abbreviatedCount == 1)
                parsedAddress.StreetType = allPossibleStreetTypes.First(); //possibleAbbreviatedStreetTypes.First();
            //else if (fullsCount == 1)
            //    parsedAddress.StreetType = possibleFullStreetTypes.First();
            else if (allCount >= 2 && allCount <= 3)
                parsedAddress.StreetType = allPossibleStreetTypes.Last();
            else if (allCount >= 2) // TODO: Remove this clause!
                parsedAddress.StreetType = allPossibleStreetTypes.First();
            else
                throw new AddressParsingException(container.CompleteAddressToParse);
        }

        private void CountPossibleStreetTypes()
        {
            abbreviatedCount = possibleAbbreviatedStreetTypes.Count;
            fullsCount = possibleFullStreetTypes.Count;
            allCount = allPossibleStreetTypes.Count;
        }

        protected override void FindPossibleMatch(AddressPartResult part)
        {
            if (MatchesAbbreviatedStreetType(part.Value))
            {
                possibleAbbreviatedStreetTypes.Add(part);
                allPossibleStreetTypes.Add(part);
            }
            else if (MatchesFullStreetType(part.Value))
            {
                possibleFullStreetTypes.Add(part);
                allPossibleStreetTypes.Add(part);
            }
        }

        public bool MatchesFullStreetType(string value)
        {
            return streetTypes
                .Exists(s => s.Full.Equals(
                    value,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        public bool MatchesAbbreviatedStreetType(string value)
        {
            // TODO: We can pull them out of a config file or database.
            var streetTypes = new List<string>()
                {
                    "ALY",
                    "AVE",
                    "BLVD",
                    "CIR",
                    "CT",
                    "DR",
                    "HWY",
                    "KY",
                    "LN",
                    "PKWY",
                    "PL",
                    "RD",
                    "SQ",
                    "ST",
                    "TER",
                    "WY",
                };

            return streetTypes
                .Exists(s => s.Equals(
                    value,
                    StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
