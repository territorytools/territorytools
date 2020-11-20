using System;
using System.Collections.Generic;
using System.Linq;

namespace TerritoryTools.Common.AddressParser.Smart
{
    public class CityNameMatcher
    {
        readonly List<CityName> cities = new List<CityName>();

        public class CityName
        {
            public string[] Words { get; set; }
            public int WordCount { get; set; }
        }

        public CityNameMatcher(List<string> names)
        {
            var list = new List<CityName>();
            foreach (string city in names)
            {
                var words = city
                    .Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                list.Add(
                    new CityName() { 
                        Words = words, 
                        WordCount = words.Length });
            }

            // Start with longer valid city names first
            cities = list.OrderByDescending(c => c.WordCount).ToList();
        }

        public string[] FindCityName(List<string> words)
        {
            foreach (var city in cities)
            {
                if(words.Count >= city.WordCount)
                {
                    bool match = true;
                    int indexShift = words.Count - city.WordCount;
                    for (int i = 0; i < city.WordCount; i++)
                    {
                        if (!string.Equals(city.Words[i], words[i + indexShift], StringComparison.OrdinalIgnoreCase))
                        {
                            // If a word doesn't match, break to the next city
                            match = false;
                            break;
                        }
                    }

                    // If no words mismatched and broke out of the for loop 
                    // then all words matched
                    if (match)
                    {
                        return city.Words;
                    }
                }
            }

            return new string[0];
        }
    }
}
