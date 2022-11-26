using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms;

public class LoginForm
{
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
}
