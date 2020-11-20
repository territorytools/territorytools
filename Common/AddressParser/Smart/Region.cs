using System;
using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Region
    {
        public const string Defaults = "AL,AK,AZ,AR,CA,CO,CT,DE,FL,GA,HI,ID,IL,IN,IA,KS,KY,LA,ME,MD,MA,MI,MN,MS,MO,MT,NE,NV,NH,NJ,NM,NY,NC,ND,OH,OK,OR,PA,RI,SC,SD,TN,TX,UT,VT,VA,WA,WV,WI,WY";
        
        public string Code { get; set; }

        public override string ToString()
        {
            return Code;
        }

        public static List<string> Split(string text)
        {
            var list = new List<string>();
            foreach (string t in text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                list.Add(t.ToUpper());
            }

            return list;
        }
    }
}
