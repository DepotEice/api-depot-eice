namespace API.DepotEice.UIL.Models
{
    public class UserModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User main email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User school email address defined by the school
        /// </summary>
        public string? SchoolEmail { get; set; }

        /// <summary>
        /// Email confirmation verification
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// User first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User gender
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// User birth date
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// User mobile phone number
        /// </summary>
        public string? MobileNumber { get; set; }

        /// <summary>
        /// User fix phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User account is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The creation date of the account
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The update date and time of the account
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The delete date and time of the account
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// The list of roles the users has
        /// </summary>
        public IEnumerable<RoleModel> Roles { get; set; } = Enumerable.Empty<RoleModel>();

    }
}
