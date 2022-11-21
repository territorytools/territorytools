using TerritoryTools.Alba.Controllers.Nominatim;
using Controllers.AlbaServer;
using Controllers.Nominatim;
using System.Text.Json;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class NominatimGeocodeAddress
    {
        private IClientView view;
        private NominatimClient client;

        public NominatimGeocodeAddress(IClientView view, NominatimClient client)
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
            
            var url = NominatimUrlBuilder.GeocodeAddress(address);
            var resultString = client.DownloadString(url);
            // TODO: Need to geocode

            GeocodedAddress[] result = JsonSerializer.Deserialize<GeocodedAddress[]>(resultString);

            if(result.Length == 0)
            {
                return null;
            }

            var a = result[0];

            double.TryParse(a.lat, out double lat);
            double.TryParse(a.lon, out double lon);

            return new Coordinate
            {
                Latitude = lat,
                Longitude = lon
            };
        }
    }
}
