using System;
using System.ComponentModel.DataAnnotations;

namespace TerritoryTools.Identity.Service.Dtos
{
    public record UserDto(
        Guid Id,
        string GivenName,
        string FamilyName,
        string Username,
        string Email,
        string Mobile,
        string Group,
        string SubGroup,
        bool IsActive,
        DateTimeOffset CreatedDate
    );

    public record UpdateUserDto(
        [Required][EmailAddress] string Email,
        string GivenName,
        string FamilyName,
        string Mobile,
        bool IsActive,
        string Group,
        string SubGroup
    );
}