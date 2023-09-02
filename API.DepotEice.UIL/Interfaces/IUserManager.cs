using API.DepotEice.UIL.Data;

namespace API.DepotEice.UIL.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Get current user's ID
        /// </summary>
        /// <returns>The ID of the currently connected user. <c>null</c> Otherwise</returns>
        string? GetCurrentUserId { get; }

        /// <summary>
        /// Check if the user has the role <see cref="RolesData.DIRECTION_ROLE"/> in his claims
        /// </summary>
        /// <returns>
        /// <c>true</c> If the user has the role <see cref="RolesData.DIRECTION_ROLE"/>. 
        /// false Otherwise
        /// </returns>
        bool IsDirection { get; }

        /// <summary>
        /// Check if the current user has a role in his claims
        /// </summary>
        /// <param name="role">
        /// The name of the role to check
        /// </param>
        /// <returns>
        /// <see cref="bool"/> true if the user has the role, false otherwise
        /// </returns>
        bool IsInRole(string role);
    }
}
