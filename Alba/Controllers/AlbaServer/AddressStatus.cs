using System.Collections.Generic;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public enum AddressStatus
    {
        New = 1,
        Valid = 2,
        DoNotCall = 3,
        Moved = 4,
        Duplicate = 5,
        NotValid = 6
    }

    public static class AddressStatusText
    {
        public readonly static Dictionary<string, int> Status
            = new Dictionary<string, int>()
        {
            { "New", (int)AddressStatus.New },
            { "Valid", (int)AddressStatus.Valid },
            { "Do not call", (int)AddressStatus.DoNotCall },
            { "Moved", (int)AddressStatus.Moved },
            { "Duplicate", (int)AddressStatus.Duplicate },
            { "Not valid", (int)AddressStatus.NotValid }
        };
    }
}
