namespace API.DepotEice.UIL.Models.Forms
{
    public class RegisterForm
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public IFormFile? ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
