using System.Text.RegularExpressions;

namespace TerritoryTools.Entities
{
    public class StringNormalizer
    {
        public static string NormalizeCharacters(string input)
        {
            if(input == null)
            {
                return null;
            }

            return Regex.Replace(input, @"[^-0-9A-Z,#\/]", " ", RegexOptions.IgnoreCase);
        }

        public static string NormalizeWhiteSpace(string input)
        {
            if (input == null)
            {
                return null;
            }

            return Regex.Replace(input, @"\s+", " ");
        }

        public static string SetOrdinalSuffixesToLowerCase(string input)
        {
            var pattern = @"((\d+)(st|nd|rd|th))";
            var evaluator = new MatchEvaluator((m) => m.Captures[0].Value.ToLower());

            return Regex.Replace(input, pattern, evaluator, RegexOptions.IgnoreCase);
        }
    }
}
