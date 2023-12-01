using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

internal class RoleAuthenticationHandler : AuthenticationHandler<RoleAuthenticationOpts> {
    public static string SchemeName = "Gyozo";

    public RoleAuthenticationHandler(IOptionsMonitor<RoleAuthenticationOpts> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) {

    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        // Read the token from request headers/cookies
        // Check that it's a valid session, depending on your implementation


        // If the session is valid, return success:
        string? role = Request.Headers["role"];

        if (role != null) return GenerateUserInfo(role);

        // If the token is missing or the session is invalid, return failure:
        return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
    }

    private static Task<AuthenticateResult> GenerateUserInfo(string role) {
        var claims = new[] {
                new Claim(ClaimTypes.Name, role),
                new Claim(ClaimTypes.Role, role)
            };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}