using EMerx.Entities;
using MongoDB.Bson;

namespace EMerx.Repositories.UserRepository;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    
    Task<User?> GetUserByEmail(string email);
    
    Task<User?> GetUserById(ObjectId id);
    
    Task CreateUser(User user);
    
    Task UpdateUser(User user);
    
    Task DeleteUser(ObjectId id);
}