using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleCommentService
    {
        IEnumerable<ArticleCommentDto> GetAll(int articleId);
        ArticleCommentDto? Create(int articleId, string userId, ArticleCommentDto data);
        ArticleCommentDto? GetById(int id);
        ArticleCommentDto? Update(int articleId, string userId, ArticleCommentDto data);
        bool Delete(int id);
    }
}
