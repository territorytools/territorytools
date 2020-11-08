using System;
using System.Collections.Generic;
using System.Text;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Street
    {
        public string Number { get; set; }
        public StreetName Name { get; set; } = new StreetName();
    }
}
