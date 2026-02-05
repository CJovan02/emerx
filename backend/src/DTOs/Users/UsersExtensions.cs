using EMerx.DTOs.Users.Response;
using EMerx.Entities;

namespace EMerx.DTOs.Users;

public static class UsersExtensions
{
    public static UserResponse ToResponse(this User domain)
    {
        return new UserResponse
        {
            Id = domain.Id,
            Name = domain.Name,
            Surname = domain.Surname,
            Email = domain.Email,
            Address = domain.Address,
        };
    }
}