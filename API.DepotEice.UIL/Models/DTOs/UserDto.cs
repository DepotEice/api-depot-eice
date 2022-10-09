namespace API.DepotEice.UIL.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string Password { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;
    public string SecurityStamp { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public IEnumerable<RoleDto>? Roles { get; set; }
    public IEnumerable<UserTokenDto>? UserTokens { get; set; }
}
