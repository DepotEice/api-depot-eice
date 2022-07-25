namespace API.DepotEice.UIL.Models.Forms
{
    public class ArticleModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Pinned { get; set; }
        public UserModel User { get; set; }
    }
}
