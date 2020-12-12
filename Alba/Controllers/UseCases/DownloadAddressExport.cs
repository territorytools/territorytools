using TerritoryTools.Alba.Controllers.AlbaServer;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class DownloadAddressExport
    {
        private AlbaConnection client;

        public DownloadAddressExport(AlbaConnection client)
        {
            this.client = client;
        }

        public void SaveAs(string fileName, int accountId)
        {
            var resultString = client.DownloadString(
                RelativeUrlBuilder.ExportAllAddresses(accountId));

            string text = AddressExportParser.Parse(resultString);

            TextFileGateway.Save(fileName, text);
        }
    }
}
