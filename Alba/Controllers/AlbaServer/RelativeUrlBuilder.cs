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

        public static string SearchAddresses(int accountId, int addressesPerPage, string searchText)
        {
            return ExportAddresses(
                accountId: accountId,
                addressesPerPage: addressesPerPage,
                searchText: searchText);
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
                $"&tid={territoryId}" +
                $"&lid={lid}&display=1%2C2%2C3%2C4%2C5%2C6" +
                $"&onlyun=false&q={HttpUtility.UrlEncode(searchText)}" +
                $"&sort=id&order=desc&lat=&lng=";
        }

        public static string GetTerritoryAssignments(string search = "")
        {
            return $"/ts?mod=assigned&cmd=search&q={search}&sort=number&order=asc" +
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

        /// <summary>
        /// Saves an existing territory.  This method also will reassign 
        /// addresses within it's borders to this territory.
        /// </summary>
        /// <param name="territory">A territory with borders, which must have 
        /// a non-zero Id</param>
        /// <returns>The URI, ready to send.</returns>
        public static string SaveTerritoryWithBorder(AlbaTerritoryBorder territory)
        {
            if(territory == null)
            {
                throw new ArgumentNullException(nameof(territory));
            }

            if(territory.Id == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(territory.Id), 
                    "Territory ID cannot be zero");
            }

            return $"/ts?mod=territories&cmd=save" +
                $"&id={territory.Id}" +
                AppendValuesFrom(territory);
        }

        public static string AddUser(AddUserRequest request)
        {
            return $"/ts?mod=users&cmd=userAdd&" +
                $"user_name={HttpUtility.UrlEncode(request.UserName)}&" +
                $"user_real_name={HttpUtility.UrlEncode(request.UserFullName)}&" +
                $"user_email={HttpUtility.UrlEncode(request.UserEmail)}&" +
                $"user_telephone={HttpUtility.UrlEncode(request.UserTelephone)}&" +
                $"pw1={HttpUtility.UrlEncode(request.Password)}&" +
                $"pw2={HttpUtility.UrlEncode(request.Password)}&" +
                $"user_role={(int)request.UserRole}&" +
                $"welcome={request.SendWelcomeEmail.ToString().ToLower()}";
        }

        public static string AddTerritoryWithBorder(AlbaTerritoryBorder territory)
        {
            return @"/ts?mod=territories&cmd=add" +
                AppendValuesFrom(territory);
        }

        public static string AssignTerritory(
            int territoryId, 
            int userId, 
            DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd");

            return $"/ts?mod=assigned&cmd=assign" +
                $"&id={territoryId}" + 
                $"&date={dateString}" +
                $"&user={userId}";
        }

        public static string UnassignTerritory(int territoryId)
        {
            return $"/ts?mod=assigned&cmd=unassign&id={territoryId}";
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
            return $"/ts?id=r1&mod=import&cmd=add" +
                $"&Address_ID={address.Address_ID}" +
                $"&Territory_ID={address.Territory_ID}" +
                $"&Language={HttpUtility.UrlEncode(address.Language)}" +
                $"&Status={HttpUtility.UrlEncode(address.Status)}" +
                $"&Name={HttpUtility.UrlEncode(address.Name)}" +
                $"&Suite={HttpUtility.UrlEncode(address.Suite)}" +
                $"&Address={HttpUtility.UrlEncode(address.Address)}" +
                $"&City={HttpUtility.UrlEncode(address.City)}" +
                $"&Province={HttpUtility.UrlEncode(address.Province)}" +
                $"&Postal_code={HttpUtility.UrlEncode(address.Postal_code)}" +
                $"&Country={HttpUtility.UrlEncode(address.Country)}" +
                $"&Latitude={address.Latitude}" +
                $"&Longitude={address.Longitude}" +
                $"&Telephone={HttpUtility.UrlEncode(address.Telephone)}" +
                $"&Notes={HttpUtility.UrlEncode(address.Notes)}" +
                $"&Notes_private={HttpUtility.UrlEncode(address.Notes_private)}";
        }

        public static string SaveAddressNotePrivate(AlbaAddressImport address)
        {
            return $"/ts?mod=addresses&cmd=save" +
                $"&id={address.Address_ID}" +
                $"&notes_private={HttpUtility.UrlEncode(address.Notes_private)}";
        }

        public static string AddAddress(AlbaAddressSave address)
        {
            string formatted = $"/ts?mod=addresses&cmd=add" +
                $"&id={address.Address_ID}" + // TODO: Is this ignored? Remove it if it is
                AppendValuesFrom(address); 

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

            return $"/ts?mod=addresses&cmd=save" +
                $"&id={address.Address_ID}" +
                AppendValuesFrom(address);
        }

        public static string GetLanguages()
        {
            return $"/addresses2";
        }

        public static string SetTerritoryCompleted(
            int territoryId, 
            DateTime completed, 
            bool allCompleted = false)
        {
            if (territoryId == 0)
            {
                throw new ArgumentException(
                    $"Territory ID cannot be zero.",
                    nameof(territoryId));
            }

            return $"/ts?mod=assigned&cmd=completed" +
                $"&id={territoryId}" +
                $"&date={completed.ToString("yyyy-MM-dd")} +" +
                $"&ac={allCompleted.ToString().ToLower()}";
        }

        static string AppendValuesFrom(AlbaTerritoryBorder territory)
        {
            return $"&kind={(int)territory.Kind}" +
                $"&number={HttpUtility.UrlEncode(territory.Number)}" +
                $"&notes={HttpUtility.UrlEncode(territory.Notes)}" +
                $"&description={HttpUtility.UrlEncode(territory.Description)}" +
                $"&border={HttpUtility.UrlEncode(CoordinatesFrom(territory))}";
        }

        static string AppendValuesFrom(AlbaAddressSave address)
        {
            return $"&lat={address.Latitude}" +
               $"&lng={address.Longitude}" +
               $"&territory_id={address.Territory_ID ?? 0}" + // This is used for borderless territories
               $"&status={address.StatusId}" +
               $"&language_id={address.LanguageId}" +
               $"&full_name={HttpUtility.UrlEncode(address.Name)}" +
               $"&suite={HttpUtility.UrlEncode(address.Suite)}" +
               $"&address={HttpUtility.UrlEncode(address.Address)}" +
               $"&city={HttpUtility.UrlEncode(address.City)}" +
               $"&province={HttpUtility.UrlEncode(address.Province)}" +
               $"&country={HttpUtility.UrlEncode(address.Country)}" +
               $"&postcode={HttpUtility.UrlEncode(address.Postal_code)}" +
               $"&telephone={HttpUtility.UrlEncode(address.Telephone)}" +
               $"&notes={HttpUtility.UrlEncode(address.Notes)}" +
               $"&notes_private={HttpUtility.UrlEncode(address.Notes_private)}";
        }

        static string CoordinatesFrom(AlbaTerritoryBorder territory)
        {
            var coordinates = new List<string>();

            //if (territory.Border.Vertices.Count > 50)
            {
                System.Diagnostics.Debug.WriteLine($"This border ({territory.Number}) has 50 or more vertices Alba cannot handle more than that.");
                //Console.WriteLine($"This border ({territory.Number}) has 50 or more vertices Alba cannot handle more than that.");
                //throw new UserException($"This border ({territory.Number}) has 50 or more vertices Alba cannot handle more than that.");
            }

            foreach (Vertex v in territory.Border.Vertices)
                coordinates.Add($"{v.Latitude} {v.Longitude}");

            return string.Join(",", coordinates);
        }
    }
}
