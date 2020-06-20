using TerritoryTools.Alba.Controllers.AlbaServer;
using Controllers.AlbaServer;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class ImportAddress
    {
        private IClientView view;
        private AuthorizationClient client;
        private int delay;

        public ImportAddress(IClientView view, AuthorizationClient client, int delay)
        {
            this.view = view;
            this.client = client;
            this.delay = delay;
        }

        public void Upload(string path)
        {
            if (client.BasePath == null)
            {
                view.ShowMessageBox("You are not logged on to Alba.  Please Logon.");
                return;
            }

           // string path = view.OpenKmlFileDialog("csv");

            if (string.IsNullOrWhiteSpace(path))
                return;

            using (var reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                var addresses = csv.GetRecords<AlbaAddressImport>();
                foreach (var address in addresses)
                {
                    var url = RelativeUrlBuilder.ImportAddress(address);
                    var resultString = client.DownloadString(url);
                    // TODO: Need to geocode

                }
            }
        }
    }
}
