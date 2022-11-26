using Microsoft.AspNetCore.Authorization;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.AuthorizationAttributes
{
    public class HasRoleRequirement : IAuthorizationRequirement
    {
        public RolesEnum Role { get; set; }
        public bool AndAbove { get; set; }

        public HasRoleRequirement(RolesEnum role, bool andAbove)
        {
            Role = role;
            AndAbove = andAbove;
        }
    }
}
