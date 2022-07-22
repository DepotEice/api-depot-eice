namespace API.DepotEice.UIL.Models
{
    public class LoggedInUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
