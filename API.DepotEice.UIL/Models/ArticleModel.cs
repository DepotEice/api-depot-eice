namespace API.DepotEice.UIL.Models
{
    /// <summary>
    /// The article model
    /// </summary>
    public class ArticleModel
    {
        /// <summary>
        /// The id of the article
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The main image of the article
        /// </summary>
        public int MainImageId { get; set; }

        /// <summary>
        /// The title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body of the article
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// The creation date of the article
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The last update date of the article
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The deletion date of the article
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Is the article pinned to the top of the list
        /// </summary>
        public bool Pinned { get; set; }

        /// <summary>
        /// The user id of the creator of the article
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
