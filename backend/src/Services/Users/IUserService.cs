using EMerx.DTOs.Id;
using EMerx.DTOs.Users;
using EMerx.DTOs.Users.Request;
using EMerx.Entities;
using EMerx.ResultPattern;
using MongoDB.Bson;

namespace EMerx.Services.Users;

public interface IUserService
{
    Task<Result<User>> GetById(IdRequest request);

    Task<Result<User>> GetByFirebaseUid(string firebaseUid);

    Task<Result<User>> RegisterAsync(RegisterUser registerUser);

    Task<Result> DeleteAsync(IdRequest request);
}