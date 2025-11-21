namespace EMerx.Repositories.AuthRepository;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password);

    Task DeleteUserAsync(string uid);
}