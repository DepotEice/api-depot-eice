namespace API.DepotEice.DAL.Entities;

/// <summary>
/// Represent the <c>Schedules</c> table in the database
/// </summary>
public class ScheduleEntity
{
    /// <summary>
    /// Represent the <c>Id</c> column in the database
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Represent the <c>Title</c> column in the database
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Represent the <c>Details</c> column in the database
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Represent the <c>StartsAt</c> column in the database
    /// </summary>
    public DateTime StartAt { get; set; }

    /// <summary>
    /// Represent the <c>EndsAt</c> column in the database
    /// </summary>
    public DateTime EndAt { get; set; }

    /// <summary>
    /// Represent the <c>ModuleId</c> column and foreign key in the database
    /// </summary>
    public int ModuleId { get; set; }
}
