using System;
using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Address
    {
        List<string> parts { get; set; } = new List<string>();

        public Street Street { get; set; } = new Street();
        public Unit Unit { get; set; } = new Unit();
        public City City { get; set; } = new City();
        public Region Region { get; set; } = new Region();
        public Postal Postal { get; set; } = new Postal();
        public string FailedAddress { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public bool SameAs(Address other)
        {
            return SameRequired(Street.Number, other.Street.Number)
                && Same(Street.Name.DirectionalPrefix, other.Street.Name.DirectionalPrefix)
                && SameRequired(Street.Name.Name, other.Street.Name.Name)
                && (SameRequired(Street.Name.StreetTypePrefix, other.Street.Name.StreetTypePrefix)
                    || SameRequired(Street.Name.StreetType, other.Street.Name.StreetType))
                && Same(Street.Name.DirectionalSuffix, other.Street.Name.DirectionalSuffix)
                && Same(Unit.Number, other.Unit.Number)
                && ((SameRequired(City.Name, other.City.Name)
                        && SameRequired(Region.Code, other.Region.Code))
                    || SameRequired(Postal.Code, other.Postal.Code));
        }

        public override string ToString()
        {
            if(!string.IsNullOrWhiteSpace(FailedAddress))
            {
                return FailedAddress;
            }

            parts.Clear();
            parts.Add(Street.ToString());
            parts.Add(Unit.ToString());
            parts.Add(City.ToString());
            parts.Add(Region.ToString());
            parts.Add(Postal.ToString());

            var notEmptyParts = new List<string>();
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    notEmptyParts.Add(part);
                }
            }

            return string.Join(" ", notEmptyParts);
        }

        bool Same(string first, string second)
        {
            return (string.IsNullOrWhiteSpace(first) 
                && string.IsNullOrWhiteSpace(second))
                || string.Equals(first?.Trim(), second?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        bool SameRequired(string first, string second)
        {
            return !string.IsNullOrWhiteSpace(first)
                && !string.IsNullOrWhiteSpace(second)
                && string.Equals(first?.Trim(), second?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
