using System;

namespace TerritoryTools.Alba.PowerShell
{
    public class MissingConnectionException : Exception
    {
        public override string Message =>
            "Use Get-AlbaConnection to set a default connection";
    }
}
