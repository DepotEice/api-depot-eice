using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IArticleRepository : IRepositoryBase<ArticleEntity, int>
    {
        bool PinArticle(int id, bool isPinned);
    }
}
