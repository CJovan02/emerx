using EMerx_backend.Entities;
using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EMerx_backend.Repositories.UserRepository;

public class UserRepository(MongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users = context.Users;

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _users.Find(u => true).ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserById(ObjectId id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateUser(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUser(User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteUser(ObjectId id)
    {
        await  _users.DeleteOneAsync(u => u.Id == id);
    }
}