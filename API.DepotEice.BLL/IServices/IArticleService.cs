using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleService
    {
        ArticleDto? CreateArticle(ArticleDto article);
        bool DeleteArticle(int id);
        IEnumerable<ArticleDto> GetArticles();
        ArticleDto? GetArticle(int id);
        bool PinArticle(int id, bool isPinned);
    }
}
