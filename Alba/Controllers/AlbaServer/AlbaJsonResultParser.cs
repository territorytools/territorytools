using System;
using Newtonsoft.Json.Linq;

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

            var nodes = JObject.Parse(value);
            var html = nodes.SelectToken("data.html") as JObject;

            var text = html.Property(property).Value.ToString();

            return text;
        }
    }
}
