using Microsoft.AspNetCore.Authorization;

namespace HelloOIDC.Auth {
    public class RoleRequirement : IAuthorizationRequirement {
        public string Role { get; set; }

        public RoleRequirement(string role) { Role = role; }
    }
}
