using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using System.Security.Claims;

namespace API.DepotEice.UIL.Managers
{
    public class UserManager : IUserManager
    {
        private readonly HttpContext _httpContext;

        public UserManager(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            _httpContext = httpContext;
        }

        /// <summary>
        /// Get current user's ID
        /// </summary>
        /// <returns>The ID of the currently connected user. <c>null</c> Otherwise</returns>
        public string? GetCurrentUserId
        {
            get
            {
                return _httpContext.User.FindFirstValue(ClaimTypes.Sid);
            }
        }

        /// <summary>
        /// Check if the user has the role <see cref="RolesData.DIRECTION_ROLE"/> in his claims
        /// </summary>
        /// <returns>
        /// <c>true</c> If the user has the role <see cref="RolesData.DIRECTION_ROLE"/>. 
        /// false Otherwise
        /// </returns>
        public bool IsDirection
        {
            get
            {
                return _httpContext.User
                    .FindAll(ClaimTypes.Role)
                    .Any(r => r.Value.Equals(RolesData.DIRECTION_ROLE));
            }
        }
    }
}
