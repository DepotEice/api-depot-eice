using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// Form to create a new passsword for the user
    /// </summary>
    public class PasswordForm
    {
        /// <summary>
        /// ID of the user to whom the password is changed
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "User's ID is required to change the password!")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The password is required !")]
        [RegularExpression(
            "(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_+]).{8,20}$",
            ErrorMessage = "The password must be 8 to 20 characters long, contains lower and uppercase and at " +
                                "least one special character!")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password
        /// </summary>
        [Compare(nameof(Password), ErrorMessage = "The passwords doesn't match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
