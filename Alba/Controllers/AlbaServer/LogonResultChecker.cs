using System;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class LogonResultChecker
    {
        public static void CheckForErrors(LogonResult result)
        {
            // Three Possible Errors Returned:
            // {"log":["Query OK"],"error":["Account name and\/or user name are unknown. Typo perhaps?"]}
            // { "error":["Invalid keys",{"an":"dd","us":"d","k2":"128dc45a5ffe04f7ea0f6ab683df07849b903485"}]}
            // {"log":["Query OK","Fetch OK"],"error":["Incorrect password."]}

            if (LogonErrorEquals(result, "Account name and/or user name are unknown. Typo perhaps?"))
                throw new AuthorizationException("Account name or user are unknown.");
            else if (LogonErrorEquals(result, "Invalid keys"))
                throw new AuthorizationException("Invalid keys!  No such user or account.");
            else if (LogonErrorEquals(result, "Incorrect password."))
                throw new AuthorizationException("Incorrect password.");
            else if (!result.authenticated)
                throw new AuthorizationException("User Logon Failed");
        }

        private static bool LogonErrorEquals(LogonResult logonResult, string errorMessage)
        {
            return logonResult.error != null
                && logonResult.error.Length > 0
                && string.Equals(
                    logonResult.error[0] as string,
                    errorMessage,
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}
