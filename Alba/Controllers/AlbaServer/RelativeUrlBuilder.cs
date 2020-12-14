using System.Collections.Generic;
using System.Web;
using TerritoryTools.Alba.Controllers.Models;
using Controllers.AlbaServer;
using System;

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

        public static string GetAllTerritoriesWithBorders()
        {
            return @"/ts?mod=territories&cmd=search&kinds%5B%5D=0" +
                "&kinds%5B%5D=1&kinds%5B%5D=2&q=&sort=number&order=asc";
        }

        public static string ExportAllAddresses(int accountId)
        {
            if (accountId == 0)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return ExportAddresses(accountId);
        }

        public static string ExportAddresses(
            int accountId, 
            int territoryId = 0,
            bool export = true,
            int addressesPerPage = 10,
            int lid = 0,
            string searchText = "")
        {
            if (accountId == 0)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return $"/ts?mod=addresses&cmd=search" + 
                $"&acids={accountId}" + 
                $"&exp={export.ToString().ToLower()}" +
                $"&npp={addressesPerPage}&cp=1" + 
                $"&tid={territoryId}&lid={lid}&display=1%2C2%2C3%2C4%2C5%2C6" +
                $"&onlyun=false&q={HttpUtility.UrlEncode(searchText)}&sort=id&order=desc&lat=&lng=";
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

        public static string DeleteAddress(int addressId)
        {
            if(addressId == 0)
            {
                throw new ArgumentNullException(nameof(addressId));
            }

            return $"/ts?mod=addresses&cmd=delete&id={addressId}";
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

        public static string AddAddress(AlbaAddressSave address)
        {
            string formatted = $"/ts?mod=addresses&cmd=add" +
                $"&id={address.Address_ID}" +
                $"&lat={address.Latitude}" +
                $"&lng={address.Longitude}" +
                $"&territory_id={address.Territory_ID ?? 0}" +
                $"&status={address.StatusId}" +
                $"&language_id={address.LanguageId}" +
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

            return HttpUtility.UrlEncode(formatted);
        }

        public static string UpdateAddress(AlbaAddressSave address)
        {
            if(address.Address_ID == null || address.Address_ID == 0)
            {
                throw new ArgumentException(
                    $"Address ID cannot be null or zero.", 
                    nameof(address.Address_ID));
            }

            string formatted = $"/ts?mod=addresses&cmd=save" +
                $"&id={address.Address_ID}" +
                $"&lat={address.Latitude}" +
                $"&lng={address.Longitude}" +
                $"&territory_id={address.Territory_ID ?? 0}" +
                $"&status={address.StatusId}" +
                $"&language_id={address.LanguageId}" +
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

            return HttpUtility.UrlEncode(formatted);
        }

        public static string GetLanguages()
        {
            return $"/addresses2";
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
