namespace TerritoryTools.Entities
{
    public enum AddressPartType
    {
        /// <summary>
        /// If nothing else is matched, Name is returned.
        /// </summary>
        Name,
        /// <summary>
        /// The street number (or house number) portion, including fractions
        /// like: 100-1/2 Main St
        /// or: 100B Main Street
        /// It also will match unit numbers and postal codes.
        /// </summary>
        Number,
        Fraction,
        /// <summary>
        /// This is the directional part of an address, like Northeast, NE, N.
        /// It can also be detected if it is part of a street name like: North St
        /// </summary>
        Direction,
        StreetType,
        UnitType,
        State,
        //PostalCode?
        UnitNumber,
        SingleLetter
    }
}
