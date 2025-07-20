using EMerx_backend.configuration;
using EMerx_backend.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Repositories.UserRepository;

public class UserRepository(MongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users = context.Users;

    public async Task<IEnumerable<User>> GetUsers()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(ObjectId id)
    {
        throw new NotImplementedException();
    }

    public async Task CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUser(ObjectId id)
    {
        throw new NotImplementedException();
    }
}