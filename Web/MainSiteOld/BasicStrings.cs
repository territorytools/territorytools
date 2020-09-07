using System;

namespace TerritoryTools.Web.MainSite
{
    public static class BasicStrings
    {
        public static bool StringsEqual(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}