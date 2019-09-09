using Newtonsoft.Json.Linq;

namespace AlbaClient.AlbaServer
{
    public class AddressExportParser
    {
        public static string Parse(string value)
        {
            var nodes = JObject.Parse(value);
            var exp = nodes.SelectToken("data") as JObject;

            var text = exp.Property("exp").Value.ToString();

            return text;
        }
    }
}
