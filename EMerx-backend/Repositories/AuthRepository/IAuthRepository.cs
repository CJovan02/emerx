namespace EMerx_backend.Repositories.AuthRepository;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password);
}