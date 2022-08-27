namespace API.DepotEice.BLL.Dtos
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPinned { get; set; }
        public UserDto User { get; set; }
        public string UserId { get; set; }

        // public IEnumerable<ArticleCommentDto> ArticleComments { get; set; }
    }
}
