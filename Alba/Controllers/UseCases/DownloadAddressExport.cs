using Alba.Controllers.AlbaServer;
using Alba.Controllers;

namespace Alba.Controllers.UseCases
{
    public class DownloadAddressExport
    {
        private AuthorizationClient client;

        public DownloadAddressExport(AuthorizationClient client)
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
