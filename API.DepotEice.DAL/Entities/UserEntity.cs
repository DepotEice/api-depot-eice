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
        public DateOnly BirthDate { get; set; }

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

        /// <summary>
        /// Instanciate an object <see cref="UserEntity"/> with all its properties
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="email">User's email</param>
        /// <param name="normalizedEmail">User's normalized email (<paramref name="email"/> in 
        /// uppercase)</param>
        /// <param name="password">User's password hash</param>
        /// <param name="salt">App's default salt</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="profilePicture">User's profile picture path in the file system</param>
        /// <param name="birthDate">User's birth date</param>
        /// <param name="concurrencyStamp">User's concurrency stamp</param>
        /// <param name="securityStamp">User's security stamp</param>
        /// <param name="createdAt"><c>Users</c> field creation date and time</param>
        /// <param name="updatedAt"><c>Users</c> field update date and time. The parameter can be 
        /// null</param>
        /// <param name="deletedAt"><c>Users</c> field deletion date and time. The parameter can be
        /// null</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public UserEntity(string id, string email, string normalizedEmail, string password,
            string salt, string firstName, string lastName, string profilePicture,
            DateOnly birthDate, string concurrencyStamp, string securityStamp, bool isActive,
            DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrEmpty(profilePicture))
            {
                throw new ArgumentNullException(nameof(profilePicture));
            }

            DateTime now = DateTime.Now;

            if (birthDate > new DateOnly(now.Year, now.Month, now.Day))
            {
                throw new ArgumentOutOfRangeException(nameof(birthDate));
            }

            if (createdAt > now)
            {
                throw new ArgumentOutOfRangeException(nameof(createdAt));
            }

            if (string.IsNullOrEmpty(concurrencyStamp))
            {
                throw new ArgumentNullException(nameof(concurrencyStamp));
            }

            if (string.IsNullOrEmpty(securityStamp))
            {
                throw new ArgumentNullException(nameof(securityStamp));
            }

            if (updatedAt is not null)
            {
                if (updatedAt < createdAt)
                {
                    throw new ArgumentOutOfRangeException(nameof(updatedAt));
                }
            }

            if (deletedAt is not null)
            {
                if (deletedAt < createdAt)
                {
                    throw new ArgumentOutOfRangeException(nameof(deletedAt));
                }
            }

            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            Password = password;
            Salt = salt;
            FirstName = firstName;
            LastName = lastName;
            ProfilePicture = profilePicture;
            BirthDate = birthDate;
            ConcurrencyStamp = concurrencyStamp;
            SecurityStamp = securityStamp;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
        }

        /// <summary>
        /// Instanciate an object <see cref="UserEntity"/> with required properties for <c>User</c>
        /// creation
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's not hashed password</param>
        /// <param name="salt">App's salt</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="profilePicture">User's profile picture</param>
        /// <param name="birthDate">User's date and birth</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public UserEntity(string email, string password, string salt, string firstName,
            string lastName, string profilePicture, DateOnly birthDate)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrEmpty(profilePicture))
            {
                throw new ArgumentNullException(nameof(profilePicture));
            }

            DateTime now = DateTime.Now;

            if (birthDate > new DateOnly(now.Year, now.Month, now.Day))
            {
                throw new ArgumentOutOfRangeException(nameof(birthDate));
            }

            Id = Guid.NewGuid().ToString();
            Email = email;
            NormalizedEmail = email.ToUpper();
            Password = password;
            Salt = salt;
            // TODO : Hash password here or in the database ?
            FirstName = firstName;
            LastName = lastName;
            ProfilePicture = profilePicture;
            BirthDate = birthDate;
            ConcurrencyStamp = Guid.NewGuid().ToString();
            SecurityStamp = Guid.NewGuid().ToString();
            IsActive = false;
            CreatedAt = DateTime.Now;
        }

        public UserEntity(string id, string email, string normalizedEmail, string firstName,
            string lastName, string profilePicture, DateOnly birthDate, string concurrencyStamp,
            string securityStamp, bool isActive, DateTime createdAt, DateTime? updatedAt,
            DateTime? deletedAt)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrEmpty(profilePicture))
            {
                throw new ArgumentNullException(nameof(profilePicture));
            }

            if (birthDate.Year > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(birthDate));
            }

            if (string.IsNullOrEmpty(concurrencyStamp))
            {
                throw new ArgumentNullException(nameof(concurrencyStamp));
            }

            if (string.IsNullOrEmpty(securityStamp))
            {
                throw new ArgumentNullException(nameof(securityStamp));
            }

            if (createdAt > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(createdAt));
            }

            if (updatedAt is not null)
            {
                if (updatedAt < createdAt)
                {
                    throw new ArgumentOutOfRangeException(nameof(updatedAt));
                }
            }

            if (deletedAt is not null)
            {
                if (deletedAt < createdAt)
                {
                    throw new ArgumentOutOfRangeException(nameof(deletedAt));
                }
            }

            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            Password = string.Empty;
            Salt = string.Empty;
            FirstName = firstName;
            LastName = lastName;
            ProfilePicture = profilePicture;
            BirthDate = birthDate;
            ConcurrencyStamp = concurrencyStamp;
            SecurityStamp = securityStamp;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
        }
    }
}
