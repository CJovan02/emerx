using FirebaseAdmin.Auth;

namespace EMerx_backend.Repositories.AuthRepository;

public class AuthRepository : IAuthRepository
{
    private readonly FirebaseAuth _firebaseAuth = FirebaseAuth.DefaultInstance;
    
    public async Task<string> RegisterAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
        };

        var userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }
}