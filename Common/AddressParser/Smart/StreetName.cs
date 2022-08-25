using System.Collections.Generic;
using st = TerritoryTools.Common.AddressParser.Smart.StreetType;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class StreetName
    {
        List<string> parts { get; set; } = new List<string>();

        public string NamePrefix { get; set; } = string.Empty;
        public string DirectionalPrefix { get; set; } = string.Empty;
        public string StreetTypePrefix { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string StreetType { get; set; } = string.Empty;
        public string DirectionalSuffix { get; set; } = string.Empty;

        public override string ToString()
        {
            parts.Clear();
            parts.Add(NamePrefix);
            parts.Add(DirectionalPrefix);
            parts.Add(StreetTypePrefix);
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
