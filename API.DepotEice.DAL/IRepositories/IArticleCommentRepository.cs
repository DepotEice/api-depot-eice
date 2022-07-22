using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IArticleCommentRepository : IRepositoryBase<ArticleCommentEntity, int>
    {
        IEnumerable<ArticleCommentEntity> GetArticleComments(int articleId);
    }
}
