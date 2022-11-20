using System;
using System.Text.Json;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AlbaJsonResultParser
    {
        const string signedOutErrorMessage = "Sorry, you have been signed out.";

        public static string ParseDataHtml(string value, string property)
        {
            if (string.IsNullOrWhiteSpace(value)
                || value.StartsWith(signedOutErrorMessage))
            {
                throw new Exception(signedOutErrorMessage);
            }

            return JsonDocument.Parse(value).RootElement.GetProperty("data").GetProperty("html").GetProperty(property).GetString();
        }
    }
}
