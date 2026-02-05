using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using EMerx.Entities;
using EMerx.ResultPattern;

namespace EMerx.Services.Users;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(IdRequest request);

    Task<Result<UserResponse>> GetByFirebaseUidAsync(string firebaseUid);

    Task<Result<UserResponse>> RegisterAsync(RegisterUser registerUser);

    Task<Result> GrantAdminRoleAsync(string email);

    Task<Result> RemoveAdminRoleAsync(string email);

    Task<Result> DeleteAsync(IdRequest request);
}