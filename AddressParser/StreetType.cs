using System;
using System.Collections.Generic;

namespace TerritoryTools.Entities
{
    public class StreetType
    {
        public static List<StreetType> From(string[] types)
        {
            var output = new List<StreetType>();

            foreach (string type in types)
                output.Add(new StreetType(type));

            return output;
        }

        public StreetType(string full)
        {
            if (string.IsNullOrWhiteSpace(full))
                throw new ArgumentException("The parameter 'full' cannot be blank or empty.");

            Full = full;
        }

        public string Full;
        public string Abbreviation;

        public static IEnumerable<StreetType> Parse(string unParsedStreetTypes)
        {
            var streetTypes = new List<StreetType>();

            foreach (string streetTypePair in unParsedStreetTypes.Split(','))
                streetTypes.Add(new StreetType(streetTypePair.Split(':')[0]) { Abbreviation = streetTypePair.Split(':')[1] });

            return streetTypes;
        }
    }
}
