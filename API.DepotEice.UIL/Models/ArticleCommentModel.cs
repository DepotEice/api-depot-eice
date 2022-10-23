namespace API.DepotEice.UIL.Models
{
    /// <summary>
    /// Article comment model to return to the user
    /// </summary>
    public class ArticleCommentModel
    {
        /// <summary>
        /// Article comment's ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Article comment's notation (From 0 to 5)
        /// </summary>
        public int Note { get; set; }

        /// <summary>
        /// Article comment's review message
        /// </summary>
        public string Review { get; set; } = string.Empty;

        /// <summary>
        /// ID of the User who created the article comment
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// ID of the article to which the comment is linked
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// The date and time when the comment was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time when the comment was updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The date and time when the comment was deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
