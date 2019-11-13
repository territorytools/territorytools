﻿using System;
using Newtonsoft.Json;

namespace AlbaClient.AlbaServer
{
    public class LogonResultChecker
    {
        public static void CheckForErrors(string result)
        {
            // Three Possible Errors Returned:
            // {"log":["Query OK"],"error":["Account name and\/or user name are unknown. Typo perhaps?"]}
            // { "error":["Invalid keys",{"an":"dd","us":"d","k2":"128dc45a5ffe04f7ea0f6ab683df07849b903485"}]}
            // {"log":["Query OK","Fetch OK"],"error":["Incorrect password."]}

            LogonResult logonResult = JsonConvert.DeserializeObject<LogonResult>(result);

            if (LogonErrorEquals(logonResult, "Account name and/or user name are unknown. Typo perhaps?"))
                throw new AuthorizationException("Account name or user are unknown.");
            else if (LogonErrorEquals(logonResult, "Invalid keys"))
                throw new AuthorizationException("Invalid keys!  No such user or account.");
            else if (LogonErrorEquals(logonResult, "Incorrect password."))
                throw new AuthorizationException("Incorrect password.");
            else if (!logonResult.authenticated)
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