﻿using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using System.Security.Claims;

namespace API.DepotEice.UIL.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly HttpContext _httpContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserManager(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor is null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            if (httpContextAccessor.HttpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));
            }

            _httpContext = httpContextAccessor.HttpContext;
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

        /// <summary>
        /// Check if the current user has a role in his claims
        /// </summary>
        /// <param name="role">
        /// The name of the role to check
        /// </param>
        /// <returns>
        /// <see cref="bool"/> true if the user has the role, false otherwise
        /// </returns>
        public bool IsInRole(string role)
        {
            return _httpContext.User.Claims.Any(c => c.Value.Equals(role));
        }
    }
}
