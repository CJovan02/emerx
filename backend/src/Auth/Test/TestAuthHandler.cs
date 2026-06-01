using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EMerx.Auth;

// Used during api-testing.
// Since some endpoints require valid JWT in order to work and
// there is no way to acquire JWT using the Firebase SDK (auth service this server uses)
// For testing purposes we inject fake claims in order to test the API.
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var adminUser = TestUsers.Admin;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, ((int)adminUser.Role).ToString()),
            new Claim(ClaimTypes.NameIdentifier, adminUser.Name),
            new Claim(ClaimTypes.Email, adminUser.Email),
            new Claim("user_id", adminUser.Uid)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}