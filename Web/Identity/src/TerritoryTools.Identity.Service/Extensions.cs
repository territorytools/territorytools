using TerritoryTools.Identity.Service.Dtos;
using TerritoryTools.Identity.Service.Entities;

namespace TerritoryTools.Identity.Service
{
    public static class Extensions
    {
        public static UserDto AsDto(this ApplicationUser user)
        {
            return new UserDto(
                user.Id,
                user.GivenName,
                user.FamilyName,
                user.UserName, 
                user.Email, 
                user.Mobile,
                user.Group,
                user.SubGroup,
                user.IsActive,
                user.CreatedOn);
        }
    }
}