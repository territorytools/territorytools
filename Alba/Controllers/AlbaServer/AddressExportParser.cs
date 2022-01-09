using Newtonsoft.Json.Linq;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AddressExportParser
    {
        public static string Parse(string value)
        {
            var nodes = JObject.Parse(value);
            var exp = nodes.SelectToken("data") as JObject;
            if(!exp.ContainsKey("exp"))
            {
                return string.Empty;
            }

            var text = exp.Property("exp").Value.ToString();

            return text;
        }
    }
}
