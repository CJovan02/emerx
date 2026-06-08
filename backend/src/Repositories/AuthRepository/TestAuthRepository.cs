using EMerx.Auth;
using EMerx.Common.Exceptions;

namespace EMerx.Repositories.AuthRepository;

public class TestAuthRepository : IAuthRepository
{
    private readonly Dictionary<string, AuthUser> _users =
        new()
        {
            [TestUsers.Admin.Uid] = TestUsers.Admin,
            [TestUsers.User.Uid] = TestUsers.User
        };

    public Task<string> GetUserUidByEmailAsync(string email)
    {
        var user = _users.Values.FirstOrDefault(x => x.Email == email);
        if (user is null)
            throw new UserNotFoundByEmail($"Auth user with email: {email} not found");

        return Task.FromResult(user.Uid);
    }

    public Task<string> RegisterAsync(string email, string password)
    {
        var uid = Guid.NewGuid().ToString();

        var user = new AuthUser(uid, email, email, Roles.User);

        _users[uid] = user;

        return Task.FromResult(uid);
    }

    public Task DeleteUserAsync(string uid)
    {
        _users.Remove(uid);
        return Task.CompletedTask;
    }

    public Task GrantAdminRoleAsync(string uid)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAdminRoleAsync(string uid)
    {
        return Task.CompletedTask;
    }
}