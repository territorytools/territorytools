using System.Collections.Generic;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class Unit
    {
        List<string> parts { get; set; } = new List<string>();

        public string Type { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        public override string ToString()
        {
            parts.Clear();
            if (Type == "#")
            {
                parts.Add($"{Type}{Number}");
            }
            else
            {
                parts.Add(Type);
                parts.Add(Number);
            }

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
