namespace API.DepotEice.UIL.Models.Forms
{
    public class UserForm
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public DateOnly BirthDate { get; set; }
        public bool IsActive { get; set; }
    }
}
