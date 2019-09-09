using AlbaClient.AlbaServer;

namespace AlbaClient.Controllers.UseCases
{
    public class DownloadAddresses
    {
        private AuthorizationClient client;

        public DownloadAddresses(AuthorizationClient client)
        {
            this.client = client;
        }

        public void SaveAs(string fileName)
        {
            var resultString = client.DownloadString(
                RelativeUrlBuilder.ExportAllAddresses());

            string text = AddressExportParser.Parse(resultString);

            TextFileGateway.Save(fileName, text);
        }
    }
}
