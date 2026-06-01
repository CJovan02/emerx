namespace EMerx.Repositories.AuthRepository;

public interface IAuthRepository
{
    Task<string> GetUserUidByEmailAsync(string email);

    Task<string> RegisterAsync(string email, string password);

    Task DeleteUserAsync(string uid);

    Task GrantAdminRoleAsync(string uid);

    Task RemoveAdminRoleAsync(string uid);
}