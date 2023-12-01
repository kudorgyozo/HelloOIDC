using HelloOIDC.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Security.Claims;

class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement> {
    private readonly ILogger<RoleAuthorizationHandler> _logger;

    public RoleAuthorizationHandler(ILogger<RoleAuthorizationHandler> logger) {
        _logger = logger;
    }

    // Check whether a given MinimumAgeRequirement is satisfied or not for a particular
    // context.
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement) {
        // Log as a warning so that it's very clear in sample output which authorization
        // policies(and requirements/handlers) are in use.
        _logger.LogWarning("Evaluating authorization requirement for role == {role}", requirement.Role);

        // Check the user's age.
        var roleClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Role);
        if (roleClaim != null) {

            // If the user meets the age criterion, mark the authorization requirement
            // succeeded.
            if (roleClaim.Value == requirement.Role) {
                _logger.LogInformation("Role authorization requirement {role} satisfied", requirement.Role);
                context.Succeed(requirement);
            } else {
                _logger.LogInformation("Current user's Role claim {userRole} does not satisfy the role authorization requirement {requiredRole}",
                    roleClaim.Value, requirement.Role);
            }
        } else {
            _logger.LogInformation("No Role claim present");
        }

        return Task.CompletedTask;
    }
}