using System.Text.Json;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class AddressExportParser
    {
        public static string Parse(string value)
        {
            var nodes = JsonDocument.Parse(value);
            if(nodes.RootElement.TryGetProperty("data", out JsonElement data)
                && data.TryGetProperty("exp", out JsonElement exp))
            {
                return exp.GetString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
