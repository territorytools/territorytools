namespace TerritoryTools.Web.Alba.Service
{
    public record UserDto(
        string UserName,
        string Name,
        string Email,
        string AlbaRole
    );
}