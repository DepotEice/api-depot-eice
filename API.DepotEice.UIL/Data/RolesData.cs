namespace API.DepotEice.UIL.Data;

/// <summary>
/// Static class containing the roles
/// </summary>
public static class RolesData
{
    /// <summary>
    /// String of the guest role
    /// </summary>
    public const string GUEST_ROLE = "Guest";

    /// <summary>
    /// String of the student role
    /// </summary>
    public const string STUDENT_ROLE = "Student";

    /// <summary>
    /// String of the teacher role
    /// </summary>
    public const string TEACHER_ROLE = "Teacher";

    /// <summary>
    /// String of the direction role
    /// </summary>
    public const string DIRECTION_ROLE = "Direction";

    /// <summary>
    /// Array of the different roles
    /// </summary>
    public static readonly string[] ROLES = new string[] { GUEST_ROLE, STUDENT_ROLE, TEACHER_ROLE, DIRECTION_ROLE };

    /// <summary>
    /// Enum of the static roles
    /// </summary>
    public enum RolesEnum
    {
        /// <summary>
        /// Guest role
        /// </summary>
        GUEST,

        /// <summary>
        /// Student role
        /// </summary>
        STUDENT,

        /// <summary>
        /// Teacher role
        /// </summary>
        TEACHER,

        /// <summary>
        /// Direction role
        /// </summary>
        DIRECTION
    }
}
