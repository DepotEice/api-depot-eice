using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Mapper;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize("IsConnected")]
public class ArticlesController : ControllerBase
{
    private const string NOTEXIST = "The selected item does not exist ! Please try again or with another one.";
    private const string ERROR = "Something went wrong ! Please contact the administrator ...";

    private readonly IArticleRepository _articleRepository;
    private readonly IArticleCommentRepository _articleCommentRepository;

    public ArticlesController(IArticleRepository articleRepository, IArticleCommentRepository articleCommentRepository)
    {
        _articleRepository = articleRepository;
        _articleCommentRepository = articleCommentRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<ArticleModel> articles = _articleRepository.GetAll().Select(x => x.Map<ArticleModel>());
            return Ok(articles);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult Get(int id)
    {
        try
        {
            ArticleModel? article = _articleRepository.GetByKey(id)?.Map<ArticleModel>();
            if (article == null)
                return NotFound();

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] ArticleForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            ArticleEntity entity = form.Map<ArticleEntity>();
            entity.UserId = GetUserId();

            int articleId = _articleRepository.Create(entity);
            if (articleId <= 0)
                return BadRequest(nameof(articleId));

            ArticleModel? article = _articleRepository.GetByKey(articleId)?.Map<ArticleModel>();
            if (article == null)
                return NotFound(NOTEXIST);

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ArticleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ArticleEntity entity = form.Map<ArticleEntity>();
            entity.UserId = GetUserId();

            bool result = _articleRepository.Update(id, entity);
            if (!result)
                return BadRequest(nameof(result));

            ArticleModel? article = _articleRepository.GetByKey(id).Map<ArticleModel>();

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            bool result = _articleRepository.Delete(id);

            if (!result)
                return BadRequest(ERROR);

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/Comments")]
    [AllowAnonymous]
    public IActionResult GetComments(int id)
    {
        try
        {
            IEnumerable<ArticleModel> comments = _articleCommentRepository
                .GetArticleComments(id)
                .Select(x => x.Map<ArticleModel>());

            return Ok(comments);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}/Comments")]
    public IActionResult PostComment(int id, [FromBody] CommentForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (!ArticleExists(id))
                return NotFound(NOTEXIST);

            var entity = form.Map<ArticleCommentEntity>();
            entity.UserId = GetUserId();
            entity.ArticleId = id;

            int commentId = _articleCommentRepository.Create(entity);
            if (commentId <= 0)
                return BadRequest(nameof(commentId));

            CommentModel? comment = _articleCommentRepository.GetByKey(commentId)?.Map<CommentModel>();
            if (comment == null)
                return NotFound(NOTEXIST);

            return Ok(comment);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}/Comments/{cId}")]
    public IActionResult PutComment(int id, int cId, [FromBody] CommentForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (!ArticleExists(id))
                return NotFound(NOTEXIST);

            var entity = form.Map<ArticleCommentEntity>();
            entity.ArticleId = id;
            entity.UserId= GetUserId();

            bool result = _articleCommentRepository.Update(cId, entity);
            if (!result)
                return BadRequest(nameof(result));

            CommentModel? comment = _articleCommentRepository.GetByKey(cId)?.Map<CommentModel>();
            if (comment == null)
                return NotFound(nameof(comment));

            return Ok(comment);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}/Comments/{cId}")]
    public IActionResult DeleteComment(int id, int cId)
    {
        try
        {
            if (!ArticleExists(id))
                return NotFound(NOTEXIST);

            bool result = _articleCommentRepository.Delete(cId);

            if (!result)
                return BadRequest(ERROR);

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private bool ArticleExists(int id)
    {
        ArticleModel? article = _articleRepository.GetByKey(id)?.Map<ArticleModel>();

        if (article == null)
            return false;
        else
            return true;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.Sid).Value;
}
