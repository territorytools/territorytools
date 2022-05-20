using System.Linq;
using TerritoryTools.Entities;
using TerritoryTools.Web.Data;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoryUserService
    {
        TerritryUserServiceUser GetUserFrom(string userName);
    }

    public class TerritryUserServiceUser
    {
        public string NormalizedEmail { get; set; }
        public string Name { get; set; }
    }

    public class TerritoryUserService : ITerritoryUserService
    {
        MainDbContext _database;

        public TerritoryUserService(MainDbContext database)
        {
            _database = database;
        }

        public TerritryUserServiceUser GetUserFrom(string userName)
        {
            string normalizedUserName = $"{userName}".ToUpper();
            if(string.IsNullOrWhiteSpace(normalizedUserName))
                return new TerritryUserServiceUser
                {
                    Name = "MISSING"
                };

            var territoryUser = _database
                .TerritoryUser
                .FirstOrDefault(u => u.Email == userName || u.Email == normalizedUserName);

            if (territoryUser == null)
            {
                return new TerritryUserServiceUser
                {
                    Name = "MISSING"
                };
            }

            return new TerritryUserServiceUser
            {
                NormalizedEmail = normalizedUserName,
                Name = $"{territoryUser.GivenName} {territoryUser.Surname}".Trim()
            };
        }
    }
}
