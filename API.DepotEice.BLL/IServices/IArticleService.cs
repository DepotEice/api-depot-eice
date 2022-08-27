using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleService
    {
        ArticleDto? CreateArticle(ArticleDto article);
        bool DeleteArticle(int id);
        ArticleDto? GetArticle(int id);
        IEnumerable<ArticleDto> GetArticles();
        bool PinArticle(int id, bool isPinned);
    }
}