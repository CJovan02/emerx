using EMerx.DTOs.Users;
using EMerx.Entities;
using EMerx.Repositories.AuthRepository;
using EMerx.Repositories.UserRepository;
using EMerx.ResultPattern;
using EMerx.ResultPattern.Errors;
using MongoDB.Bson;

namespace EMerx.Services.UserService;

public class UserService(IUserRepository userRepository, IAuthRepository authRepository) : IUserService
{
    public async Task<Result<User>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        // calls the auth repository to try and create firebase auth account
        var uid = await authRepository.RegisterAsync(registerUserDto.Email, registerUserDto.Password);

        // if it's successful, we also create the dabatase entry
        var user = new User
        {
            Email = registerUserDto.Email,
            Name = registerUserDto.Name,
            Surname = registerUserDto.Surname,
            FirebaseUid = uid,
        };

        await userRepository.CreateUser(user);

        return Result<User>.Success(user);
    }

    /// <summary>
    /// We query the user in db with the id = userId
    /// Then we grab his firebaseUid, and using that we call authRepo to delete firebase user.
    ///
    /// It could be faster if we had the same id for the firebase auth user and databse user id, but delete user is not
    /// a common operation, and it's usually slow because it also deletes all data connected to that user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<Result> DeleteUserAsync(ObjectId userId)
    {
        // We find the user with the specified id
        var dbUser = await userRepository.GetUserById(userId);
        
        if (dbUser is null)
            return Result.Failure(UserErrors.NotFound(userId));
        
        // We delete the user with extracted firebaseId
        await authRepository.DeleteUserAsync(dbUser.FirebaseUid);
        
        // Then we delete user and user data
        await userRepository.DeleteUser(userId);

        return Result.Success();
    }
}