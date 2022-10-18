using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize("IsConnected")]
public class ArticlesController : ControllerBase
{
    private const string NOTEXIST = "The selected item does not exist ! Please try again or with another one.";
    private const string ERROR = "Something went wrong ! Please contact the administrator ...";

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleCommentRepository _articleCommentRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="articleRepository"></param>
    /// <param name="articleCommentRepository"></param>
    /// <param name="userRepository"></param>
    public ArticlesController(ILogger<ArticlesController> logger, IMapper mapper, IArticleRepository articleRepository,
        IArticleCommentRepository articleCommentRepository, IUserRepository userRepository)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (articleRepository is null)
        {
            throw new ArgumentNullException(nameof(articleRepository));
        }

        if (articleCommentRepository is null)
        {
            throw new ArgumentNullException(nameof(articleCommentRepository));
        }

        if (userRepository is null)
        {
            throw new ArgumentNullException(nameof(userRepository));
        }

        _logger = logger;
        _mapper = mapper;
        _articleRepository = articleRepository;
        _articleCommentRepository = articleCommentRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Get all articles
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> with a list of articles if the operation succeeded without any errors.
    /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred.
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Get()
    {
        try
        {
            List<ArticleModel> articles = new List<ArticleModel>();

            IEnumerable<ArticleEntity> articlesFromRepo = _articleRepository.GetAll();

            foreach (ArticleEntity articleFromRepo in articlesFromRepo)
            {
                UserEntity userFromRepo = _userRepository.GetByKey(articleFromRepo.UserId);

                ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

                article.User = _mapper.Map<UserModel>(userFromRepo);

                articles.Add(article);
            }

            return Ok(articles);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Get an Article based on its id
    /// </summary>
    /// <param name="id">ID of the article to retrieve</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> with the article object if the article is correctly retrieved.
    /// <see cref="StatusCodes.Status404NotFound"/> If the article does not exist.
    /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred.
    /// </returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult Get(int id)
    {
        try
        {
            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound();
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(articleFromRepo.UserId);

            if (userFromRepo is null)
            {
                return NotFound("Article author does not exist!");
            }

            ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

            article.User = _mapper.Map<UserModel>(userFromRepo);

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // TODO : Limit this endpoint to Teachers / Direction
    /// <summary>
    /// Create a new article
    /// </summary>
    /// <param name="form">The Article to create</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> and the object article if the creation went well
    /// <see cref="StatusCodes.Status400BadRequest"/> if an error occurred during the process
    /// </returns>
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
            {
                return BadRequest(nameof(articleId));
            }

            ArticleModel? article = _articleRepository.GetByKey(articleId)?.Map<ArticleModel>();

            if (article is null)
            {
                return NotFound(NOTEXIST);
            }

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ArticleForm form)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ArticleEntity entity = form.Map<ArticleEntity>();

            entity.UserId = GetUserId();

            bool result = _articleRepository.Update(id, entity);

            if (!result)
            {
                return BadRequest(nameof(result));
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(entity.Id);

            if (articleFromRepo is null)
            {
                return NotFound("Updated article does not exist anymore");
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(articleFromRepo.UserId);

            if (userFromRepo is null)
            {
                return NotFound("Article author doesn't exist!");
            }

            ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

            article.User = _mapper.Map<UserModel>(userFromRepo);

            return Ok(article);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="form"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cId"></param>
    /// <param name="form"></param>
    /// <returns></returns>
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
            entity.UserId = GetUserId();

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cId"></param>
    /// <returns></returns>
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

    private string? GetUserId() => User.FindFirst(ClaimTypes.Sid)?.Value;
}
