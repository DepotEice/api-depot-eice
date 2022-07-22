using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IUserRepository : IRepositoryBase<UserEntity, string>
    {
        /// <summary>
        /// Set the <see cref="UserEntity.IsActive"/> property to <paramref name="isActive"/>'s 
        /// value
        /// </summary>
        /// <param name="id">
        /// The <see cref="UserEntity.Id"/> of the <see cref="UserEntity"/> to activate
        /// </param>
        /// <param name="isActive">
        /// The <see cref="UserEntity.IsActive"/>'s value you want
        /// </param>
        /// <returns>
        /// <c>true</c> If at least one row was affected. <c>false</c> Otherwise
        /// </returns>
        bool ActivateUser(string id, bool isActive);

        /// <summary>
        /// Update <see cref="UserEntity.Password"/> column in the database
        /// </summary>
        /// <param name="id"><see cref="UserEntity.Id"/>'s value</param>
        /// <param name="oldPassword">User's old password</param>
        /// <param name="newPassword">User's new password</param>
        /// <param name="salt">Application's secret salt</param>
        /// <returns>
        /// <c>true</c> If user's password has been updated. <c>false</c> Otherwise
        /// </returns>
        bool UpdatePassword(string id, string oldPassword, string newPassword, string salt);
    }
}
