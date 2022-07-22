using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IUserTokenRepository : IRepositoryBase<UserTokenEntity, string>
    {
        /// <summary>
        /// Retrieve all <see cref="UserTokenEntity"/> from the database related to 
        /// <see cref="UserEntity"/>
        /// </summary>
        /// <param name="userId">
        /// <see cref="UserEntity.Id"/>'s value
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of <see cref="UserTokenEntity"/>
        /// </returns>
        IEnumerable<UserTokenEntity> GetUserTokens(string userId);
    }
}
