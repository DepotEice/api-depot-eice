namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// The form used to create or update an article
    /// </summary>
    public class ArticleForm
    {
        /// <summary>
        /// The id of the main image
        /// </summary>
        public int MainImageId { get; set; }

        /// <summary>
        /// The title of the article
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The body of the article
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Is the article pinned to the top of the list
        /// </summary>
        public bool Pinned { get; set; }
    }
}
