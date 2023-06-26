namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// Represents a form for creating or updating a user.
    /// </summary>
    public class UserForm
    {
        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the profile picture of the user.
        /// </summary>
        public IFormFile? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateOnly BirthDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }
    }

}
