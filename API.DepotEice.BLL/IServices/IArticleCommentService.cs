using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IArticleCommentService
    {
        ArticleCommentDto? CreateArticleComment(ArticleCommentDto model);
        bool DeleteArticleComment(int id);
        IEnumerable<ArticleCommentDto> GetAllArticleComments(int articleId);
        ArticleCommentDto? GetArticleComment(int id);
        ArticleCommentDto? UpdateArticleComment(ArticleCommentDto model);
    }
}
