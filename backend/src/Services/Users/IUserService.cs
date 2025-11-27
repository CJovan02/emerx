using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.Entities;
using EMerx.ResultPattern;

namespace EMerx.Services.Users;

public interface IUserService
{
    Task<Result<User>> GetByIdAsync(IdRequest request);

    Task<Result<User>> GetByFirebaseUidAsync(string firebaseUid);

    Task<Result<User>> RegisterAsync(RegisterUser registerUser);

    Task<Result> GrantAdminRoleAsync(string email);

    Task<Result> RemoveAdminRoleAsync(string email);

    Task<Result> DeleteAsync(IdRequest request);
}