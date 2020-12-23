using System;

namespace TerritoryTools.Alba.PowerShell
{
    public class MissingLanguagesException : Exception
    {
        public override string Message =>
            "Run Get-AlbaLanguage to load a list of language IDs from Alba";
    }
}
