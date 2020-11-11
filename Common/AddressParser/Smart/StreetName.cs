using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class StreetName
    {
        List<string> parts { get; set; } = new List<string>();

        public string DirectionalPrefix { get; set; }
        public string PrefixStreetType { get; set; }
        public string Name { get; set; }
        public string StreetType { get; set; }
        public string DirectionalSuffix { get; set; }

        public override string ToString()
        {
            parts.Clear();
            parts.Add(DirectionalPrefix);
            parts.Add(PrefixStreetType);
            parts.Add(Name);
            parts.Add(StreetType);
            parts.Add(DirectionalSuffix);

            var notEmptyParts = new List<string>();
            foreach(var part in parts)
            {
                if(!string.IsNullOrWhiteSpace(part))
                {
                    notEmptyParts.Add(part);
                }
            }

            return string.Join(" ", notEmptyParts);
        }
    }
}
