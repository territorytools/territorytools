using Controllers.AlbaServer;

namespace Alba.Controllers.Nominatim
{
    public class NominatimUrlBuilder
    {
        public static string GeocodeAddress(AlbaAddressImport address)
        {
            if(string.IsNullOrEmpty(address.Country))
            {
                address.Country = "United States";
            }

            string formatted = $"street={address.Address}&city={address.City}&state={address.Province}&postalcode={address.Postal_code}";
            //&country={address.Country}";
            formatted = formatted.Replace(",", "%2C").Replace(" ", "+");
            return $"search?"
                + $"{formatted}"
                + $"&format=json";
        }
    }
}
