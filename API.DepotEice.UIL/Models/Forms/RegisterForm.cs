using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// The form used for user to register
    /// </summary>
    public class RegisterForm
    {
        /// <summary>
        /// The email address of the user
        /// </summary>
        [EmailAddress(ErrorMessage = "The email address format is incorrect!")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The email address is required!")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password. Must be 8 to 20 characters long, contains lower and uppercase and 
        /// special character
        /// </summary>
        [RegularExpression("")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Must match exactly <see cref="Password"/> property
        /// </summary>
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Represent User's first name. Must be between 2 and 50 characters long
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The first name is required!")]
        [MinLength(2, ErrorMessage = "The first name must be at least 2 characters long!")]
        [MaxLength(50, ErrorMessage = "The first name must be less than 50 characters long!")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Represent User's last name. Must be between 2 and 100 characters long
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The last name is required!")]
        [MinLength(2, ErrorMessage = "The last name must be at least 2 characters longs!")]
        [MaxLength(100, ErrorMessage = "The last name must be less than 100 characters longs!")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Represent User's birthdate. The the time even if noted is useless
        /// </summary>
        [Required(ErrorMessage = "The birth date is required!")]
        public DateTime BirthDate { get; set; }
    }
}
