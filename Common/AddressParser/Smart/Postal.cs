using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Postal
    {
        List<string> parts { get; set; } = new List<string>();

        public string Code { get; set; }
        public string Extra { get; set; }

        public override string ToString()
        {
            parts.Clear();
            parts.Add(Code);
            parts.Add(Extra);

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
