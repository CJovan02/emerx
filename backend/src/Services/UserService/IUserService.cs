using EMerx.DTOs.Users;
using EMerx.DTOs.Users.Request;
using EMerx.Entities;
using EMerx.ResultPattern;
using MongoDB.Bson;

namespace EMerx.Services.UserService;

public interface IUserService
{
    Task<Result<User>> GetUserById(ObjectId id);
    
    Task<Result<User>> GetUserByFirebaseUid(string firebaseUid);
    
    Task<Result<User>> RegisterAsync(RegisterUser registerUser);

    Task<Result> DeleteUserAsync(ObjectId userId);
}