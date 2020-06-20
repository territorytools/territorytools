using TerritoryTools.Alba.Controllers.AzureMaps;
using Controllers.AlbaServer;
using Controllers.AzureMaps;
using Newtonsoft.Json;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class AzureMapsmGeocodeAddress
    {
        private IClientView view;
        private AzureMapsClient client;

        public AzureMapsmGeocodeAddress(IClientView view, AzureMapsClient client)
        {
            this.view = view;
            this.client = client;
        }
        
        public class Coordinate
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public Coordinate Geocode(AlbaAddressImport address)
        {
            if (client.BasePath == null)
            {
                view.ShowMessageBox("You do not have a base path set!");
                return null;
            }
            
            var url = AzureMapsUrlBuilder.GeocodeAddress(address);
            var resultString = client.DownloadString(url);
   
            var result = JsonConvert.DeserializeObject<GeocodeResult>(resultString);

            if(result.results.Length == 0)
            {
                return null;
            }

            var r = result.results[0];

            return new Coordinate
            {
                Latitude = r.position.lat,
                Longitude = r.position.lon
            };
        }
    }
}
