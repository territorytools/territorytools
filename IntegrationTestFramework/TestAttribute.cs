using System;
using System.Collections.Generic;

namespace TerritoryTools.IntegrationTestFramework
{
    public class TestAttribute : Attribute
    {
        public TestAttribute()
        {
            Console.WriteLine("Instantiate...");
            Stuff.Add(this);
        }

        public static List<TestAttribute> Stuff = new List<TestAttribute>();

        public override bool Match(object obj)
        {
            Console.WriteLine("Match...");
            return base.Match(obj);
        }
    }
}
