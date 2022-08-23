namespace API.DepotEice.BLL.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public IEnumerable<RoleDto>? Roles { get; set; }

        public string NormalizedEmail { get; set; } = string.Empty;
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public string SecurityStamp { get; set; } = string.Empty;

        public IEnumerable<ModuleDto>? Modules { get; set; }
        public IEnumerable<MessageDto>? Messages { get; set; }
        public IEnumerable<AppointmentDto>? Appointments { get; set; }
        public IEnumerable<UserTokenDto>? UserTokens { get; set; }
        public IEnumerable<ArticleDto>? Articles { get; set; }
        public IEnumerable<ArticleCommentDto>? ArticleComments { get; set; }
    }
}
