using EMerx.Auth;
using EMerx.Common.Exceptions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace EMerx.Repositories.AuthRepository;

public class AuthRepository : IAuthRepository
{
    private readonly FirebaseAuth _firebaseAuth = FirebaseAuth.DefaultInstance;

    public async Task<UserRecord> GetUserByUidAsync(string uid)
    {
        // In order to properly mock this error in unit tests, I need to replace FirebaseException with my own
        // Because FirebaseAuthException is internal class and I can't create its instance
        try
        {
            return await _firebaseAuth.GetUserAsync(uid);
        }
        catch (FirebaseAuthException e) when (e.ErrorCode == ErrorCode.NotFound)
        {
            throw new UserNotFoundById(uid);
        }
    }

    public async Task<UserRecord> GetUserByEmailAsync(string email)
    {
        // In order to properly mock this error in unit tests, I need to replace FirebaseException with my own
        // Because FirebaseAuthException is internal class and I can't create its instance
        try
        {
            return await _firebaseAuth.GetUserByEmailAsync(email);
        }
        catch (FirebaseAuthException e) when (e.ErrorCode == ErrorCode.NotFound)
        {
            throw new UserNotFoundByEmail(email);
        }
    }

    public async Task<string> GetUserUidByEmailAsync(string email)
    {
        // I order to properly mock this error in unit tests, I need to replace FirebaseException with my own
        // Because FirebaseAuthException is internal class and I can't create its instance
        try
        {
            return (await _firebaseAuth.GetUserByEmailAsync(email)).Uid;
        }
        catch (FirebaseAuthException e) when (e.ErrorCode == ErrorCode.NotFound)
        {
            throw new UserNotFoundByEmail(email);
        }
    }

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

    public async Task DeleteUserAsync(string uid)
    {
        try
        {
            await _firebaseAuth.DeleteUserAsync(uid);
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
        {
            throw new UserNotFoundById(uid);
        }
    }

    /// <summary>
    /// IMPORTANT: this function will overwrite the "roles" claim and add the ["Admin"] role in array 
    /// </summary>
    public async Task GrantAdminRoleAsync(string uid)
    {
        // This will just overwrite the existing roles array
        // It will remain like this until we decide to add more roles
        var adminRole = new Dictionary<string, object>
        {
            { "roles", new[] { Roles.Admin } }
        };
        await _firebaseAuth.SetCustomUserClaimsAsync(uid, adminRole);
    }

    /// <summary>
    /// IMPORTANT: this function will actually remove every role from the user
    /// </summary>
    public async Task RemoveAdminRoleAsync(string uid)
    {
        var removedRole = new Dictionary<string, object>
        {
            { "roles", null! }
        };
        await _firebaseAuth.SetCustomUserClaimsAsync(uid, removedRole);
    }
}