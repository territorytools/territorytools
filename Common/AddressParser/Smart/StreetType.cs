using System;
using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class StreetType
    {
        public static List<string> Split(string text)
        { 
            var streetTypes = new List<string>();
            foreach(string t in text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (string tt in t.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    streetTypes.Add(tt.ToUpper());
                }
            }

            return streetTypes;
        }
    }
}
