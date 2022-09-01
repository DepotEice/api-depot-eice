namespace API.DepotEice.UIL.Models;

public class LoggedInUserModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public bool IsActive { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IEnumerable<RoleModel> Roles { get; set; }
}
