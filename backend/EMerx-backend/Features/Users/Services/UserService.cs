using EMerx_backend.Features.Users.Dtos;
using EMerx_backend.Features.Users.Errors;
using EMerx_backend.Features.Users.Repositories;
using FluentResults;
using Mapster;
using MongoDB.Bson;

namespace EMerx_backend.Features.Users.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<Result<IEnumerable<UserDto>>> GetUsers()
    {
        return Result.Ok((await _userRepository.GetUsers())
            .Adapt<IEnumerable<UserDto>>());
    }

    public async Task<Result<User>> GetUserById(ObjectId id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user is null)
            return Result.Fail<User>(new UserNotFoundError(id));
        return Result.Ok(user);
    }

    public async Task<Result<UserDto>> CreateUser(RegisterUserDto userDto)
    {
        if (await _userRepository.GetUserByEmail(userDto.Email) != null)
            return Result.Fail<UserDto>(new EmailOccupiedError(userDto.Email));

        var user = userDto.Adapt<User>();
        await _userRepository.CreateUser(user);
        return Result.Ok(user.Adapt<UserDto>());
    }
    
    //will have separate dto where you couldn't update password from here
    public async Task<Result<UserDto>> UpdateUser(UserDto userDto)
    {
        var user = await _userRepository.GetUserById(userDto.Id);
        if(user is null)
            return Result.Fail<UserDto>(new UserNotFoundError(userDto.Id));
        
        //separate method
        user.Name = userDto.Name;
        user.Surname = userDto.Surname;
        user.Email = userDto.Email;
        user.Password = userDto.Password;
        user.Address = userDto.Address;
        await _userRepository.UpdateUser(user);
        
        return Result.Ok(user.Adapt<UserDto>());
    }
    
    public async Task<Result> DeleteUser(ObjectId id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user is null)
            return Result.Fail(new UserNotFoundError(id));
        return Result.Ok();
    }
}