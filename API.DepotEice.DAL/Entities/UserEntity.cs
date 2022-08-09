using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>Users</c> table in the database
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// Represent <c>Users</c> table's <c>Id</c> column
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>Email</c> column
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>NormalizedEmail</c> column
        /// </summary>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>PasswordHash</c> column
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>Salt</c> column
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>FirstName</c> column
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>LastName</c> column
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>ProfilePicture</c> column
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>BirthDate</c> column
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>ConcurrencyStamp</c> column
        /// </summary>
        public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>SecurityStamp</c> column
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>IsActive</c> column
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>CreatedAt</c> column
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>UpdatedAt</c> column. Null value is authorized
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Represent <c>Users</c> table's <c>DeletedAt</c> column. Null value is authorized
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        public UserEntity()
        {

        }
    }
}
