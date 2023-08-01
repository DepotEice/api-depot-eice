namespace API.DepotEice.DAL.Entities;

/// <summary>
/// Represent the <c>Users</c> table in the database
/// </summary>
public class UserEntity
{
    /// <summary>
    /// Represent <c>Users</c> table's <c>Id</c> column
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>ProfilePictureId</c> column
    /// </summary>
    public int? ProfilePictureId { get; set; }

    /// <summary>
    /// Represent <c>Users</c> table's <c>Email</c> column
    /// </summary>
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>NormalizedEmail</c> column
    /// </summary>
    public string? NormalizedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Email address provided by the establishment
    /// </summary>
    public string? SchoolEmail { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of the property <see cref="SchoolEmail"/>
    /// </summary>
    public string? NormalizedSchoolEmail { get; set; } = string.Empty;

    /// <summary>
    /// Represents the boolean value true or false for email address activation.
    /// </summary>
    public bool EmailConfirmed { get; set; } = false;

    /// <summary>
    /// Represent <c>Users</c> table's <c>FirstName</c> column
    /// </summary>
    public string? FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>LastName</c> column
    /// </summary>
    public string? LastName { get; set; } = string.Empty;

    public string? Gender { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>BirthDate</c> column
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// The mobile (GSM) number of the user
    /// </summary>
    public string? MobileNumber { get; set; } = string.Empty;

    /// <summary>
    /// The phone (fix) number of the user
    /// </summary>
    public string? PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>ConcurrencyStamp</c> column
    /// </summary>
    public string ConcurrencyStamp { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>SecurityStamp</c> column
    /// </summary>
    public string SecurityStamp { get; set; } = string.Empty;

    /// <summary>
    /// Represent <c>Users</c> table's <c>IsActive</c> column
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Represent <c>Users</c> table's <c>CreatedAt</c> column
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Represent <c>Users</c> table's <c>UpdatedAt</c> column. Null value is authorized
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Represent <c>Users</c> table's <c>DeletedAt</c> column. Null value is authorized
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
