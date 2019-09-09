using System;
using Newtonsoft.Json.Linq;

namespace AlbaClient.Controllers.AlbaServer
{
    public class TerritoryAssignmentParser
    {
        const string signedOutErrorMessage = "Sorry, you have been signed out.";

        public static string Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value) 
                || value.StartsWith(signedOutErrorMessage))
            {
                throw new Exception(signedOutErrorMessage);
            }

            var nodes = JObject.Parse(value);
            var html = nodes.SelectToken("data.html") as JObject;

            var text = html.Property("territories").Value.ToString();

            return text;
        }
    }
}
