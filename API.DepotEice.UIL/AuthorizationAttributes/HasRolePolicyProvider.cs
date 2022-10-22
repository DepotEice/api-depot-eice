using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.AuthorizationAttributes
{
    public class HasRolePolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX = "HasRoleAuthorize";

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(POLICY_PREFIX))
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            string[] policyNameArray = policyName.Split('.');

            if (policyNameArray.Length != 3)
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            if (!Enum.TryParse(typeof(RolesEnum), policyNameArray[1], out object? role))
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            if (role is null)
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            if (!bool.TryParse(policyNameArray[2], out bool andAbove))
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            policy.AddRequirements(new HasRoleRequirement((RolesEnum)role, andAbove));
            return Task.FromResult((AuthorizationPolicy?)policy.Build());
        }
    }
}
