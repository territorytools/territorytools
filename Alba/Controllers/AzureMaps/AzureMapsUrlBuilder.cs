using Controllers.AlbaServer;

namespace AlbaClient.AzureMaps
{
    public class AzureMapsUrlBuilder
    {
        public static string GeocodeAddress(AlbaAddressImport address)
        {
            string countryCode = "us";
            if(string.IsNullOrEmpty(address.Country))
            {
                address.Country = "United States";
            }

            string formatted = $"streetName={address.Address}&municipality={address.City}&countrySubDivision={address.Province}&postalCode={address.Postal_code}&countryCode={countryCode}";
            formatted = formatted.Replace(",", "%2C").Replace(" ", "+");
            return $"/search/address/structured/json?{formatted}";
        }
    }
}
