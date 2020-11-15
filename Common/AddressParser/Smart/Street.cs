using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Street
    {
        List<string> parts { get; set; } = new List<string>();

        public string Number { get; set; }
        public string NumberFraction { get; set; }
        public StreetName Name { get; set; } = new StreetName();

        public override string ToString()
        {
            parts.Clear();
            parts.Add(Number);
            parts.Add(Name.ToString());

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
    }
}
