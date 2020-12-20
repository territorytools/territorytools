using System;
using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    [Cmdlet(VerbsCommon.Get, nameof(AlbaLanguage))]
    [OutputType(typeof(AlbaLanguage))]
    public class GetAlbaLanguage : AlbaConnectedCmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                var downloader = new LanguageDownloader(Connection);
                var languages = downloader.GetLanguages();

                foreach (var language in languages)
                {
                    WriteObject(language);
                }

                // This almost never changes so we can persist it to an environment variable
                SessionState.PSVariable.Set(Names.AlbaLanguages.ToString(), languages);

                var languageIds = new Dictionary<string, int>();
                foreach(var language in languages)
                {
                    languageIds.Add(language.Name, language.Id);
                }

                SessionState.PSVariable.Set(Names.AlbaLanguageIds.ToString(), languageIds);
            }
            catch(Exception e)
            {
                WriteError(new ErrorRecord(e, "1", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
