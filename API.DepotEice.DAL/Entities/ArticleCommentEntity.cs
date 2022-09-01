namespace API.DepotEice.DAL.Entities;

public class ArticleCommentEntity
{
    /// <summary>
    /// Article's comment's ID 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Article's given note by the user
    /// </summary>
    public int Note { get; set; }

    /// <summary>
    /// User's review text
    /// </summary>
    public string Review { get; set; }

    /// <summary>
    /// ID of the user writing the comment
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Article's ID on which this comment is written
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Date and time at which this article's comment was created in the database
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time at which this article's comment was updated in the database.
    /// If the article comment has been created and never update, <see cref="UpdatedAt"/> will
    /// be the same as <see cref="CreatedAt"/>
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
