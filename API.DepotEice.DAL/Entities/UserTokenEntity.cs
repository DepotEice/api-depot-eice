using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>UserToken</c> table in the database
    /// </summary>
    public class UserTokenEntity
    {
        /// <summary>
        /// <see cref="UserTokenEntity"/>'s ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s type
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s value
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s delivery date and time
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s expiration date and time
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Linked <see cref="UserEntity"/>'s <see cref="UserEntity.Id"/> property
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Linked <see cref="UserEntity"/>'s <see cref="UserEntity.SecurityStamp"/> property. This
        /// property is required only if <see cref="UserTokenEntity"/> is instanciated for creation
        /// </summary>
        public string? UserSecurityStamp { get; set; }
    }
}
