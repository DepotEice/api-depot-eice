using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleService
    {
        ArticleModel? CreateArticle(ArticleModel article);
        bool DeleteArticle(int id);
        IEnumerable<ArticleModel> GetArticles();
        ArticleModel? GetArticle(int id);
        bool PinArticle(int id, bool isPinned);
    }
}
