using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleService
    {
        IEnumerable<ArticleDto> GetAll();
        ArticleDto? Create(string userId, ArticleDto data);
        ArticleDto? GetByKey(int key);
        ArticleDto? Update(int key, string userId, ArticleDto data);
        bool Delete(int key);
        bool PinArticle(int id, bool isPinned);
    }
}