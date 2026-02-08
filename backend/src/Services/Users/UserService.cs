using EMerx.DTOs.Id;
using EMerx.DTOs.Users;
using EMerx.DTOs.Users.Request;
using EMerx.DTOs.Users.Response;
using EMerx.Entities;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using FirebaseAdmin.Auth;
using MongoDB.Bson;

namespace EMerx.Services.Users;

public class UserService(IUserRepository userRepository, IAuthRepository authRepository) : IUserService
{
    public async Task<Result<UserResponse>> GetByIdAsync(IdRequest request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var user = await userRepository.GetUserById(objectId);
        if (user is null)
            return Result<UserResponse>.Failure(UserErrors.NotFound(objectId));

        return Result<UserResponse>.Success(user.ToResponse());
    }

    public async Task<Result<UserResponse>> GetByFirebaseUidAsync(string firebaseUid)
    {
        var user = await userRepository.GetUserByFirebaseUid(firebaseUid);
        if (user is null)
            return Result<UserResponse>.Failure(UserErrors.NotFound(firebaseUid));

        return Result<UserResponse>.Success(user.ToResponse());
    }

    public async Task<Result<UserResponse>> RegisterAsync(RegisterUser registerUser)
    {
        // calls the auth repository to try and create firebase auth account
        var uid = await authRepository.RegisterAsync(registerUser.Email, registerUser.Password);

        // if it's successful, we also create the database entry
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

            return Result<UserResponse>.Success(user.ToResponse());
        }
        catch (Exception)
        {
            await authRepository.DeleteUserAsync(user.FirebaseUid);
            return Result<UserResponse>.Failure(GeneralErrors.DatabaseError());
        }
    }

    public async Task<Result> GrantAdminRoleAsync(string email)
    {
        try
        {
            var user = await authRepository.GetUserByEmailAsync(email);
            await authRepository.GrantAdminRoleAsync(user.Uid);

            return Result.Success();
        }
        // Thrown if auth user with the provided email can't be found
        catch (FirebaseAuthException e)
        {
            return Result.Failure(AuthErrors.NotFoundByEmail(email));
        }
    }

    public async Task<Result> RemoveAdminRoleAsync(string email)
    {
        try
        {
            var user = await authRepository.GetUserByEmailAsync(email);
            await authRepository.RemoveAdminRoleAsync(user.Uid);

            return Result.Success();
        }
        // Thrown if auth user with the provided email can't be found
        catch (FirebaseAuthException e)
        {
            return Result.Failure(AuthErrors.NotFoundByEmail(email));
        }
    }

    /// <summary>
    /// We query the user in db with the id = userId
    /// Then we grab his firebaseUid, and using that we call authRepo to delete firebase user.
    ///
    /// It could be faster if we had the same id for the firebase auth user and database user id, but delete user is not
    /// a common operation, and it's usually slow because it also deletes all data connected to that user
    /// </summary>
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

    public async Task<Result> DeleteByFirebaseIdAsync(string firebaseUid)
    {
        // We find the user with the specified id
        var dbUser = await userRepository.GetUserByFirebaseUid(firebaseUid);

        if (dbUser is null)
            return Result.Failure(UserErrors.NotFound(firebaseUid));

        // We delete the user with extracted firebaseId
        await authRepository.DeleteUserAsync(dbUser.FirebaseUid);

        // Then we delete user and user data
        await userRepository.DeleteUser(dbUser.Id);

        return Result.Success();
    }
}