using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IArticleRepository : IRepositoryBase<int, ArticleEntity>
{
    bool ArticleExist(int id);
    bool ArticlePinDecision(int id, bool isPinned = true);
}
