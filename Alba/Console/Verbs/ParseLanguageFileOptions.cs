using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "parse-language-file", 
        HelpText = "Parse language file downloaded from Alba")]
    public class ParseLanguageFileOptions
    {
        [Option("filepath", Required = true, HelpText = "Input file path")]
        [Value(0)]
        public string FilePath { get; set; }

        [Usage(ApplicationAlias = "alba")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example(
                        "Download example", 
                        new ParseLanguageFileOptions { 
                            FilePath = "languages.html" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Parsing language file...");

            var languages = LanguageDownloader.LoadLanguagesFrom(FilePath);

            Console.WriteLine($"Languages parsed: {languages.Count}");
            foreach(var language in languages)
            {
                Console.WriteLine($"  {language.Id,4:0}: {language.Name}");
            }

            Console.WriteLine("Done");

            return 0;
        }
    }
}
