using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// Form to update the password of the user
    /// </summary>
    public class PasswordUpdateForm
    {
        /// <summary>
        /// The user's current password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "User's current password is required to change the password!")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// The user's new password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The new password is required !")]
        [RegularExpression("(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_+]).{8,20}$",
            ErrorMessage = "The new password must be 8 to 20 characters long, contains lower and uppercase and at " +
                "least one special character!")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The confirmation of the new password is required !")]
        [Compare(nameof(NewPassword), ErrorMessage = "The passwords doesn't match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
