using EMerx.DTOs.Id;
using EMerx.DTOs.Users.Request;
using EMerx.Entities;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;

namespace EMerx.Services.Users;

public class UserService(IUserRepository userRepository, IAuthRepository authRepository) : IUserService
{
    public async Task<Result<User>> GetById(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var user = await userRepository.GetUserById(objectId);
        if (user is null)
            return Result<User>.Failure(UserErrors.NotFound(objectId));

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> GetByFirebaseUid(string firebaseUid)
    {
        var user = await userRepository.GetUserByFirebaseUid(firebaseUid);
        if (user is null)
            return Result<User>.Failure(UserErrors.NotFound(firebaseUid));

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> RegisterAsync(RegisterUser registerUser)
    {
        // calls the auth repository to try and create firebase auth account
        var uid = await authRepository.RegisterAsync(registerUser.Email, registerUser.Password);

        // if it's successful, we also create the dabatase entry
        var user = new User
        {
            Email = registerUser.Email,
            Name = registerUser.Name,
            Surname = registerUser.Surname,
            FirebaseUid = uid,
        };

        try
        {
            await userRepository.CreateUser(user);

            return Result<User>.Success(user);
        }
        catch (Exception)
        {
            await authRepository.DeleteUserAsync(user.FirebaseUid);
            return Result<User>.Failure(GeneralErrors.DatabaseError());
        }
    }

    /// <summary>
    /// We query the user in db with the id = userId
    /// Then we grab his firebaseUid, and using that we call authRepo to delete firebase user.
    ///
    /// It could be faster if we had the same id for the firebase auth user and databse user id, but delete user is not
    /// a common operation, and it's usually slow because it also deletes all data connected to that user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result> DeleteAsync(IdRequest request)
    {
        // We find the user with the specified id
        var objectId = ObjectId.Parse(request.Id);
        var dbUser = await userRepository.GetUserById(objectId);

        if (dbUser is null)
            return Result.Failure(UserErrors.NotFound(objectId));

        // We delete the user with extracted firebaseId
        await authRepository.DeleteUserAsync(dbUser.FirebaseUid);

        // Then we delete user and user data
        await userRepository.DeleteUser(objectId);

        return Result.Success();
    }
}