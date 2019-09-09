using AlbaClient.AlbaServer;
using AlbaClient.Kml;
using System;
using System.Linq;
using System.Threading;

namespace AlbaClient.Controllers.UseCases
{
    public class UploadKmlFile
    {
        private IClientView view;
        private AuthorizationClient client;
        private int delay;

        public UploadKmlFile(IClientView view, AuthorizationClient client, int delay)
        {
            this.view = view;
            this.client = client;
            this.delay = delay;
        }

        public void Upload()
        {
            if (client.BasePath == null)
            {
                view.ShowMessageBox("You are not logged on to Alba.  Please Logon.");
                return;
            }

            string fileName = view.OpenKmlFileDialog("kml");

            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var kml = new KmlGateway().Load(fileName);
            var territories = new KmlToTerritoryConverter()
                .TerritoryListFrom(kml)
                .Where(t => t.Border != null && t.Border.Vertices != null && t.Border.Vertices.Count > 0)
                .ToList();

            view.AppendResultText("Uploading " + territories.Count + " territory borders...");

            foreach (var territory in territories)
            {
                int count = territory.Border.Vertices.Count;

                var url = RelativeUrlBuilder.RequestToAddNew(territory);
                var resultString = client.DownloadString(url);

                view.AppendResultText("Territory: " + territory.Number);
                view.AppendResultText(count + " vertices where uploaded..." + Environment.NewLine);
                view.AppendResultText(resultString + Environment.NewLine + Environment.NewLine);
                Thread.Sleep(delay);
            }
        }
    }
}
