using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "AlbaLanguageId")]
    [OutputType(typeof(int))]
    public class GetAlbaLanguageId : AlbaConnectedCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                List<AlbaLanguage> languages = SessionState
                    .PSVariable
                    .GetValue(Names.AlbaLanguages.ToString())
                    as List<AlbaLanguage>;

                if (languages == null)
                {
                    var downloader = new LanguageDownloader(Connection);
                    languages = downloader.GetLanguages();

                    // This almost never changes so we can persist it to an environment variable
                    SessionState.PSVariable.Set(Names.AlbaLanguages.ToString(), languages);
                }

                Dictionary<string, int> languageIds = SessionState
                    .PSVariable
                    .GetValue(Names.AlbaLanguageIds.ToString())
                    as Dictionary<string, int>;

                if (languageIds == null)
                {
                    languageIds = new Dictionary<string, int>();
                    foreach (var language in languages)
                    {
                        languageIds.Add(language.Name, language.Id);
                    }

                    SessionState.PSVariable.Set(Names.AlbaLanguageIds.ToString(), languageIds);
                }

                WriteObject(languageIds[Name]);
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
