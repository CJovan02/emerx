using FirebaseAdmin.Auth;

namespace EMerx.Repositories.AuthRepository;

public interface IAuthRepository
{
    Task<UserRecord> GetUserByUidAsync(string uid);

    Task<UserRecord> GetUserByEmailAsync(string email);
    
    Task<string> RegisterAsync(string email, string password);

    Task DeleteUserAsync(string uid);

    Task GrantAdminRoleAsync(string uid);

    Task RemoveAdminRoleAsync(string uid);
}