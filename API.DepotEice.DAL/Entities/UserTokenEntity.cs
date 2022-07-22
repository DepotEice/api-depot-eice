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
        public string Id { get; set; }

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s delivery date and time
        /// </summary>
        public DateTime DeliveryDateTime { get; set; }

        /// <summary>
        /// <see cref="UserTokenEntity"/>'s expiration date and time
        /// </summary>
        public DateTime ExpirationDateTime { get; set; }

        /// <summary>
        /// Linked <see cref="UserEntity"/>'s <see cref="UserEntity.Id"/> property
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Linked <see cref="UserEntity"/>'s <see cref="UserEntity.SecurityStamp"/> property. This
        /// property is required only if <see cref="UserTokenEntity"/> is instanciated for creation
        /// </summary>
        public string? UserSecurityStamp { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="UserTokenEntity"/> with all its properties
        /// </summary>
        /// <param name="id">Token's id</param>
        /// <param name="type">Token type</param>
        /// <param name="value">Token value</param>
        /// <param name="deliveryDateTime">Delivery date and time</param>
        /// <param name="expirationDateTime">Token's expiration date and time</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public UserTokenEntity(string id, string type, string value, DateTime deliveryDateTime,
            DateTime expirationDateTime, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (deliveryDateTime > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(deliveryDateTime));
            }

            if (expirationDateTime < deliveryDateTime)
            {
                throw new ArgumentOutOfRangeException(nameof(expirationDateTime));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = id;
            Type = type;
            Value = value;
            DeliveryDateTime = deliveryDateTime;
            ExpirationDateTime = expirationDateTime;
            UserId = userId;
        }

        /// <summary>
        /// Instanciate an object <see cref="UserTokenEntity"/> with all its property except for 
        /// <see cref="Id"/> and <see cref="DeliveryDateTime"/> that are generated
        /// </summary>
        /// <param name="type">
        /// The token type
        /// </param>
        /// <param name="expirationDatetime">
        /// Token expiration date and time
        /// </param>
        /// <param name="userId">
        /// The id of <see cref="UserEntity"/>
        /// </param>
        /// <param name="userSecurityStamp">
        /// The security stamp of the <see cref="UserEntity"/>. Usefull for the creation of the
        /// token
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public UserTokenEntity(string type, DateTime expirationDatetime,
            string userId, string userSecurityStamp)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (expirationDatetime < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(expirationDatetime));
            }

            if (string.IsNullOrEmpty(UserId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = Guid.NewGuid().ToString();
            Type = type;
            Value = string.Empty;
            DeliveryDateTime = DateTime.Now;
            ExpirationDateTime = expirationDatetime;
            UserId = userId;
            UserSecurityStamp = userSecurityStamp;
        }
    }
}
