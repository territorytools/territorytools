using System.Collections.Generic;
using System.Web;
using AlbaClient.Models;
using Controllers.AlbaServer;

namespace AlbaClient.AlbaServer
{
    public class RelativeUrlBuilder
    {
        public static string AuthenticationUrlFrom(Credentials credentials)
        {
            return @"/gk.php?"
               + "an=" + credentials.Account
               + "&us=" + credentials.User
               + "&k2=" + HashComputer.Hash(credentials.Combined);
        }

        public static string GetAllTerritories()
        {
            return @"/ts?mod=territories&cmd=search&kinds%5B%5D=0&kinds%5B%5D=1&kinds%5B%5D=2&q=&sort=number&order=asc"; 
        }

        public static string ExportAllAddresses(int accountId)
        {
            return $"/ts?mod=addresses&cmd=search&acids={accountId}&exp=true&npp=25&cp=1&tid=0&lid=0&display=1%2C2%2C3%2C4%2C5%2C6&onlyun=false&q=&sort=id&order=desc&lat=&lng=";
        }

        public static string GetTerritoryAssignments()
        {
            return @"/ts?mod=assigned&cmd=search&q=&sort=number&order=asc&av=true&so=true&tk0=true&tk1=true&tk2=true";
        }

        public static string GetTerritoryAssignmentsPage()
        {
            return @"/assigned";
        }

        public static string GetUserManagementPage()
        {
            return @"/ts?mod=users&cmd=usersSearch&q=&sort=user_name&order=asc";
        }

        public static string RequestToAddNew(Territory territory)
        {
            return @"/ts?mod=territories&cmd=add&kind=0"
                + "&number=" + HttpUtility.UrlEncode(territory.Number)
                + "&notes=" + HttpUtility.UrlEncode(territory.Notes)
                + "&description=" + HttpUtility.UrlEncode(territory.Description)
                + "&border=" + HttpUtility.UrlEncode(CoordinatesFrom(territory));
        }

        public static string GeocodeAddress(AlbaAddressImport address)
        {
            if(string.IsNullOrEmpty(address.Country))
            {
                address.Country = "United States";
            }

            string formatted = $"https://nominatim.openstreetmap.org/search?street=4069+jones+ln&city=bellingham&state=wa&postalcode=98225&format=json";
            //string formatted = $"{address.Address} {address.Suite}, {address.City}, {address.Province}, {address.Postal_code} {address.Country}";
            formatted = formatted.Replace(",", "%2C").Replace(" ", "%20");
            return $"https://api.apple-mapkit.com/v1/geocode?"
                + $"q={formatted}&"
                + $"lang=en&" 
                + $"mkjsVersion=5.39.0";
        }

        public static string ImportAddress(AlbaAddressImport address)
        {
            string formatted = $"/ts?id=r1&mod=import&cmd=add" +
                $"&Address_ID={address.Address_ID}" +
                $"&Territory_ID={address.Territory_ID}" +
                $"&Language={address.Language}" +
                $"&Status={address.Status}" +
                $"&Name={address.Name}" +
                $"&Suite={address.Suite}" +
                $"&Address={address.Address}" +
                $"&City={address.City}" +
                $"&Province={address.Province}" +
                $"&Postal_code={address.Postal_code}" +
                $"&Country={address.Country}" +
                $"&Latitude={address.Latitude}" +
                $"&Longitude={address.Longitude}" +
                $"&Telephone={address.Telephone}" +
                $"&Notes={address.Notes}" +
                $"&Notes_private={address.Notes_private}";

            return formatted.Replace(" ", "+").Replace(",", "%2C");
        }


        private static string CoordinatesFrom(Territory territory)
        {
            var coordinates = new List<string>();

            foreach (Vertex v in territory.Border.Vertices)
                coordinates.Add(v.Latitude + " " + v.Longitude);

            return string.Join(",", coordinates);
        }
    }
}
