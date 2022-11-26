using API.DepotEice.UIL.Data;
using Microsoft.AspNetCore.Authorization;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.AuthorizationAttributes
{
    public class HasRoleAuthorizeAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "HasRoleAuthorize";

        /// <summary>
        /// 
        /// </summary>
        public RolesEnum Role
        {
            get
            {
                if (string.IsNullOrEmpty(Policy))
                {
                    return default;
                }

                string[] policyArguments = Policy.Split('.');

                if (policyArguments.Length <= 0)
                {
                    throw new IndexOutOfRangeException($"{nameof(policyArguments)} length is equal or less " +
                        $"than 0");
                }

                if (Enum.TryParse(typeof(RolesEnum), policyArguments[1], out object? role))
                {
                    if (role is null)
                    {
                        throw new NullReferenceException(nameof(role));
                    }

                    return (RolesEnum)role;
                }

                throw new Exception($"Could not get {Role} property's value");
            }
            set
            {
                Policy = $"{POLICY_PREFIX}.{value}.{AndAbove}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AndAbove
        {
            get
            {
                if (string.IsNullOrEmpty(Policy))
                {
                    return default;
                }

                string[] policyArguments = Policy.Split('.');

                if (policyArguments.Length <= 0)
                {
                    throw new IndexOutOfRangeException($"{nameof(policyArguments)} length is equal or less " +
                        $"than 0");
                }

                if (bool.TryParse(policyArguments[policyArguments.Length - 1], out bool andAbove))
                {
                    return andAbove;
                }

                return default;
            }
            set
            {
                Policy = $"{POLICY_PREFIX}.{Role}.{value}";
            }
        }

        /// <summary>
        /// Instanciate the attribute by providing the role and specifying the hierarchy
        /// </summary>
        /// <param name="role">
        /// The role to which the authorization is granted
        /// </param>
        /// <param name="andAbove">
        /// Specify if the authorization also applies to the 'upper' roles. For example, if <paramref name="andAbove"/>
        /// is <c>true</c> and <paramref name="role"/> is <see cref="RolesEnum.STUDENT"/>, the authorization is granted
        /// to the <c>Student</c> role but also to the <c>Teacher</c> and <c>Direction</c> roles.
        /// </param>
        public HasRoleAuthorizeAttribute(RolesEnum role, bool andAbove = true)
        {
            Role = role;
            AndAbove = andAbove;
        }
    }
}
