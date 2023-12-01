using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelloOIDC.Auth;

class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement
{
    public const string POLICY_PREFIX = "RolePolicy";

    public RoleAuthorizeAttribute(string role) => TheRole = role;
    public string TheRole {
        get {
            return Policy!.Substring(POLICY_PREFIX.Length);
        }
        set {
            Policy = $"{POLICY_PREFIX}{value}";
        }
    }

    public IEnumerable<IAuthorizationRequirement> GetRequirements() {
        yield return this;
    }
}

