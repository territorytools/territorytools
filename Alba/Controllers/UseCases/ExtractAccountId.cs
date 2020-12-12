using Newtonsoft.Json;
using TerritoryTools.Alba.Controllers.AlbaServer;

namespace Controllers.UseCases
{
    public class ExtractAccountId
    {
        public static int ExtractFrom(string result)
        {
            var logonResult = JsonConvert.DeserializeObject<LogonResult>(result);

            return logonResult?.user?.id ?? 0;
        }
    }
}
