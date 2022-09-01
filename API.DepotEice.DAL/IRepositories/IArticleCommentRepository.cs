using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IArticleCommentRepository : IRepositoryBase<int, ArticleCommentEntity>
{
    IEnumerable<ArticleCommentEntity> GetArticleComments(int articleId);
}
