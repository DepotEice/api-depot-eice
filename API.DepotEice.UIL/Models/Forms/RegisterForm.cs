namespace API.DepotEice.UIL.Models.Forms
{
    public class RegisterForm
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Password { get; set; }
    }
}
