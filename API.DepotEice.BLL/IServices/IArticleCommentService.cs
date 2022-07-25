using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleCommentService
    {
        ArticleCommentModel? CreateArticleComment(ArticleCommentModel model);
        bool DeleteArticleComment(int id);
        IEnumerable<ArticleCommentModel> GetAllArticleComments(int articleId);
        ArticleCommentModel? GetArticleComment(int id);
        ArticleCommentModel? UpdateArticleComment(ArticleCommentModel model);
    }
}
