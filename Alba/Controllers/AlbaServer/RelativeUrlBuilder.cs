using System.Collections.Generic;
using System.Web;
using TerritoryTools.Alba.Controllers.Models;
using Controllers.AlbaServer;

namespace TerritoryTools.Alba.Controllers.AlbaServer
{
    public class RelativeUrlBuilder
    {
        public static string AuthenticationUrlFrom(Credentials credentials)
        {
            return @"/gk.php?" +
               $"an={credentials.Account}" +
               $"&us={credentials.User}" +
               $"&k2={HashComputer.Hash(credentials.Combined)}";
        }

        public static string GetAllTerritories()
        {
            return @"/ts?mod=territories&cmd=search&kinds%5B%5D=0" +
                "&kinds%5B%5D=1&kinds%5B%5D=2&q=&sort=number&order=asc";
        }

        public static string ExportAllAddresses(int accountId)
        {
            return $"/ts?mod=addresses&cmd=search&acids={accountId}&exp=true" +
                "&npp=25&cp=1&tid=0&lid=0&display=1%2C2%2C3%2C4%2C5%2C6" +
                "&onlyun=false&q=&sort=id&order=desc&lat=&lng=";
        }

        public static string GetTerritoryAssignments()
        {
            return @"/ts?mod=assigned&cmd=search&q=&sort=number&order=asc" +
                "&av=true&so=true&tk0=true&tk1=true&tk2=true";
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
            return @"/ts?mod=territories&cmd=add&kind=0" +
                $"&number={HttpUtility.UrlEncode(territory.Number)}" +
                $"&notes={HttpUtility.UrlEncode(territory.Notes)}" +
                $"&description={HttpUtility.UrlEncode(territory.Description)}" +
                $"&border={HttpUtility.UrlEncode(CoordinatesFrom(territory))}";
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

        public static string SaveAddressNotePrivate(AlbaAddressImport address)
        {
            string formatted = $"/ts?mod=addresses&cmd=save" +
                $"&id={address.Address_ID}" +
                $"&notes_private={address.Notes_private}";

            return formatted.Replace(" ", "+").Replace(",", "%2C");
        }

        public static string EditAddress(AlbaAddressImport address)
        {
            string formatted = $"/ts?mod=addresses&cmd=edit" +
                $"&lat={address.Latitude}" +
                $"&lng={address.Longitude}" +
                $"&id={address.Address_ID}";

            return formatted.Replace(" ", "+").Replace(",", "%2C");
        }

        public static string SaveAddress(AlbaAddressImport address)
        {
            // Status codes
            // 1 New
            // 2 Valid
            // 3 Do not call
            // 4 Moved
            // 5 Duplicate
            // 6 Not valid

            // Chinese language codes
            // 83 Chinese
            //5  Chiense Cantonese
            // 188 Chines Fukien
            // 4 Chiense Mandarin
            // 5 Chiense Cantonese

            string formatted = $"/ts?mod=addresses&cmd=save" +
                $"&id={address.Address_ID}" +
                $"&lat={address.Latitude}" +
                $"&lng={address.Longitude}" +
                $"&territory_id={address.Territory_ID ?? 0}" +
                $"&status=1" +
                $"&language_id=4" +
                $"&full_name={address.Name}" +
                $"&suite={address.Suite}" +
                $"&address={address.Address}" +
                $"&city={address.City}" +
                $"&province={address.Province}" +
                $"&country={address.Country}" +
                $"&postcode={address.Postal_code}" +
                $"&telephone={address.Telephone}" +
                $"&notes={address.Notes}" +
                $"&notes_private={address.Notes_private}";

            return formatted.Replace(" ", "+").Replace(",", "%2C");
        }

        private static string CoordinatesFrom(Territory territory)
        {
            var coordinates = new List<string>();

            foreach (Vertex v in territory.Border.Vertices)
                coordinates.Add($"{v.Latitude} {v.Longitude}");

            return string.Join(",", coordinates);
        }
    }
}
