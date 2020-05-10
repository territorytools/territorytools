using System.Collections.Generic;
using System.Web;
using AlbaClient.Models;

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

        public static string ExportAllAddresses()
        {
            return @"/ts?mod=addresses&cmd=search&acids=940&exp=true&npp=25&cp=1&tid=0&lid=0&display=1%2C2%2C3%2C4%2C5%2C6&onlyun=false&q=&sort=id&order=desc&lat=&lng=";
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

        private static string CoordinatesFrom(Territory territory)
        {
            var coordinates = new List<string>();

            foreach (Vertex v in territory.Border.Vertices)
                coordinates.Add(v.Latitude + " " + v.Longitude);

            return string.Join(",", coordinates);
        }
    }
}
