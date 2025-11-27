using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EMerx.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresRoleAttribute(Roles role) : Attribute, IAuthorizationFilter
{
    private readonly string _claimName = "roles";
    private readonly string _claimValue = ((int)role).ToString();

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // C# will internally map the "roles" claim into its own type (ClaimType.Role)
        // If the "roles" claim is array, it will create multiple claims with the same name but with different values
        // ex: "roles": ["admin", "user"] => { "role": "admin", "role": "user" }
        var roles = user.FindAll(ClaimTypes.Role).ToList();

        var isAdmin = roles.Any(x => x.Value == ((int)Roles.Admin).ToString());
        
        // Admin is allowed to use any endpoint
        if (isAdmin) return;

        var hasRole = roles.Any(x => x.Value == _claimValue);

        if (!hasRole)
            context.Result = new ForbidResult();
    }
}