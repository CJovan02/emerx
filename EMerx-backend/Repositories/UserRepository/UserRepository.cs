using EMerx_backend.Dto.User;
using EMerx_backend.Entities;
using EMerx_backend.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

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

    public async Task PatchUser(PatchUserDto patchUserDto)
    {
        var updates = new List<UpdateDefinition<User>>();

        if (patchUserDto.Name != null)
            updates.Add(Builders<User>.Update.Set(u => u.Name, patchUserDto.Name));
        if (patchUserDto.Surname != null)
            updates.Add(Builders<User>.Update.Set(u => u.Surname, patchUserDto.Surname));
        if (patchUserDto.Address != null)
            updates.Add(Builders<User>.Update.Set(u => u.Address, patchUserDto.Address));

        if (updates.Count == 0)
            return;

        var updateDef = Builders<User>.Update.Combine(updates);

        await _users.UpdateOneAsync(user => user.Id == patchUserDto.Id, updateDef);
    }

    public async Task DeleteUser(ObjectId id)
    {
        await  _users.DeleteOneAsync(u => u.Id == id);
    }
}