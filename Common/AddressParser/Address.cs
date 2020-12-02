using System;

namespace TerritoryTools.Entities
{
    public class Address 
    {
        static Address()
        {
            empty = new EmptyAddress();
        }

        static readonly Address empty;

        public static Address Empty 
        { 
            get { return empty; } 
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public string Number { get; set; }

        public string NumberFraction { get; set; }

        public string DirectionalPrefix { get; set; }

        public string StreetName { get; set; }

        public string StreetType { get; set; }

        public string DirectionalSuffix { get; set; }

        public string UnitType { get; set; }

        public string UnitNumber { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string PostalCodeExt { get; set; }

        public bool IsNotPhysical { get; set; }
        
        public bool StreetNameIsAfterType { get; set; }

        public string CombineStreet()
        {
            if(StreetNameIsAfterType)
            {
                return StringNormalizer
                .NormalizeWhiteSpace(
                    StringNormalizer.NormalizeCharacters(
                        $"{Number}{NumberFraction} {DirectionalPrefix} {StreetType} {StreetName} {DirectionalSuffix}"))
                .Trim();
            }

            return StringNormalizer
                .NormalizeWhiteSpace(
                    StringNormalizer.NormalizeCharacters(
                        $"{Number}{NumberFraction} {DirectionalPrefix} {StreetName} {StreetType} {DirectionalSuffix}"))
                .Trim();
        }

        public string CombineUnit()
        {
            return StringNormalizer
                .NormalizeWhiteSpace(
                    StringNormalizer.NormalizeCharacters($"{UnitNumber} {UnitType}"))
                .Trim();
        }

        public bool SameAs(Address other, SameAsOptions options = SameAsOptions.None)
        {
            return Equals(Number, other.Number)
                && Equals(NumberFraction, other.NumberFraction)
                && Equals(DirectionalPrefix, other.DirectionalPrefix)
                && Equals(StreetName, other.StreetName)
                // TODO: StreetType needs abbreviation check
                && Equals(StreetType, other.StreetType)
                && Equals(DirectionalSuffix, other.DirectionalSuffix)
                // UnitType is ignored
                && Equals(UnitNumber, other.UnitNumber)
                && (Equals(City, other.City) 
                    || (options == SameAsOptions.AssumeMissingCityIsSame 
                        && (string.IsNullOrWhiteSpace(City)
                            || string.IsNullOrWhiteSpace(other.City))))
                // PostalCode and PostalCodeExt are ignored by default
                && (options != SameAsOptions.ComparePostalCode
                    || Equals(PostalCode, other.PostalCode))
                && Equals(State, other.State);
        }

        bool Equals(string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        private class EmptyAddress : Address
        {
            public override bool IsEmpty
            {
                get { return true; }
            }
        }

        [Flags]
        public enum SameAsOptions
        {
            None,
            ComparePostalCode,
            AssumeMissingCityIsSame
        }
    }
}
