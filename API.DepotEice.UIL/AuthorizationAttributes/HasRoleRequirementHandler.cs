using API.DepotEice.UIL.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.DepotEice.UIL.AuthorizationAttributes
{
    public class HasRoleRequirementHandler : AuthorizationHandler<HasRoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, HasRoleRequirement requirement)
        {
            if (context.User is null)
            {
                return Task.CompletedTask;
            }

            IEnumerable<Claim> roles = context.User.Claims.Where(c => c.Type.Equals(ClaimTypes.Role));

            if (!roles.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var roleClaim in roles)
            {
                if (roleClaim.Value.Equals(RolesData.ROLES[(int)requirement.Role]))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            if (requirement.AndAbove)
            {
                foreach (var roleClaim in roles)
                {
                    string roleName = roleClaim.Value;

                    int roleIndex = Array.IndexOf(RolesData.ROLES, roleName);

                    if (roleIndex < 0)
                    {
                        return Task.CompletedTask;
                    }

                    if (roleIndex > (int)requirement.Role)
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
