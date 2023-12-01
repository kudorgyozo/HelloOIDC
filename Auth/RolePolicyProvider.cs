using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace HelloOIDC.Auth;

internal class RolePolicyProvider : IAuthorizationPolicyProvider {


    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() {
        var policy = new AuthorizationPolicyBuilder(RoleAuthenticationHandler.SchemeName);
        policy.RequireAuthenticatedUser();
        return Task.FromResult<AuthorizationPolicy>(policy.Build());
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() {
        var policy = new AuthorizationPolicyBuilder(RoleAuthenticationHandler.SchemeName);
        policy.RequireAuthenticatedUser();
        return Task.FromResult<AuthorizationPolicy?>(null);
    }

    // Policies are looked up by string name, so expect 'parameters' (like age)
    // to be embedded in the policy names. This is abstracted away from developers
    // by the more strongly-typed attributes derived from AuthorizeAttribute
    // (like [MinimumAgeAuthorize()] in this sample)
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) {
        if (policyName.StartsWith(RoleAuthorizeAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)) {
            var role = policyName.Substring(RoleAuthorizeAttribute.POLICY_PREFIX.Length);
            var policy = new AuthorizationPolicyBuilder(RoleAuthenticationHandler.SchemeName);
            policy.AddRequirements(new RoleRequirement(role));
            return Task.FromResult(policy?.Build());
        }

        return Task.FromResult<AuthorizationPolicy?>(null);
    }
}

