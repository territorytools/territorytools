using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.Cli.Verbs
{
    [Verb(
        "download-languages", 
        HelpText = "Download language list from Alba using your account")]
    public class DownloadLanguagesOptions : IOptionsWithRun
    {
        [Option("filepath", Required = true, HelpText = "Output file path")]
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
                        new DownloadLanguagesOptions { 
                            FilePath = "languages.html" 
                        }
                    )
                };
            }
        }

        public int Run()
        {
            Console.WriteLine("Downloading language list...");

            var client = Program.AlbaClient();

            client.Authenticate(Program.GetCredentials());

            var useCase = new LanguageDownloader(client);

            useCase.SaveAs(FilePath);

            Console.WriteLine("Parsing language list file...");

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
