using CommandLine;
using System;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "fake-ids", 
        HelpText = "Output a list of integer for testing")]
    public class FakeIdsOptions : IOptionsWithRun
    {
        [Option("start", Required = false, HelpText = "Starting number")]
        [Value(0)]
        public int Start { get; set; } = 0;

        [Option("end", Required = false, HelpText = "Ending number")]
        [Value(1)]
        public int End { get; set; } = 10;

        public int Run()
        {
            int start = Start;
            int end = End;
            if (end < start)
                end = start;

            for (int i = start; i <= end; i++)
            {
                Console.Out.WriteLine($"{i}");
            }

            return 0;
        }
    }
}
