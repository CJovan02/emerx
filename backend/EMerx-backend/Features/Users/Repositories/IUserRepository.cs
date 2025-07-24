using MongoDB.Bson;

namespace EMerx_backend.Features.Users.Repositories;
public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(ObjectId id);
    Task CreateUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(ObjectId id);
}