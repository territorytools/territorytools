using Controllers.AlbaServer;
using Controllers.AzureMaps;
using System;
using System.Text.Json;
using TerritoryTools.Alba.Controllers.AzureMaps;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.UseCases
{
    public class AzureMapsmGeocodeAddressException : Exception
    {
        public AzureMapsmGeocodeAddressException(string message) : base(message)
        {
        }
    }

    public class AzureMapsmGeocodeAddress
    {
        private AzureMapsClient client;

        public AzureMapsmGeocodeAddress(AzureMapsClient client)
        {
            this.client = client;
        }

        public AzureMapsmGeocodeAddress(string key)
        {
            client = AzureMapsClientFrom(key);
        }

        public class Coordinate
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public AlbaAddressImport Geocode(AlbaAddressImport address, bool force = false)
        {
            if (client.BasePath == null)
            {
                throw new AzureMapsmGeocodeAddressException("You do not have a base path set!");
            }

            if (force || address.Latitude == null
                || address.Longitude == null
                || address.Latitude == 0
                || address.Longitude == 0)
            {
                var coordinates = GeocodeFrom(address);
                address.Latitude = coordinates.Latitude;
                address.Longitude = coordinates.Longitude;
            }

            return address;
        }

        public Coordinate CoordinatesFrom(AlbaAddressImport address)
        {
            if (client.BasePath == null)
            {
                throw new AzureMapsmGeocodeAddressException("You do not have a base path set!");
            }

            return GeocodeFrom(address);
        }

        public static AzureMapsClient AzureMapsClientFrom(string key)
        {
            var amWebClient = new CookieWebClient();
            var amBasePath = new ApplicationBasePath(
                protocolPrefix: "https://",
                site: "atlas.microsoft.com",
                applicationPath: "/");

            var amClient = new AzureMapsClient(
               webClient: amWebClient,
               basePath: amBasePath,
               subscriptionKey: key);

            return amClient;
        }

        Coordinate GeocodeFrom(AlbaAddressImport address)
        {
            string url = AzureMapsUrlBuilder.GeocodeAddress(address);
            string resultString = client.DownloadString(url);

            GeocodeResult result = JsonSerializer
                .Deserialize<GeocodeResult>(resultString);

            if (result.results.Length == 0)
            {
                return null;
            }

            Result r = result.results[0];

            return new Coordinate
            {
                Latitude = r.position.lat,
                Longitude = r.position.lon
            };
        }
    }
}
