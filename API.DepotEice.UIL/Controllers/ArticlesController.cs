using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private const string NOTEXIST = "The selected item does not exist ! Please try again or with another one.";
    private const string ERROR = "Something went wrong ! Please contact the administrator ...";

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserManager _userManager;
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
    public ArticlesController(ILogger<ArticlesController> logger, IMapper mapper, IUserManager userManager,
        IArticleRepository articleRepository, IArticleCommentRepository articleCommentRepository,
        IUserRepository userRepository)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
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
        _userManager = userManager;
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
    //[AllowAnonymous]
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

                if (userFromRepo is null)
                {
                    return NotFound("Article creator doesn't exist!");
                }

                article.userId = userFromRepo.Id;

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

            article.userId = userFromRepo.Id;

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
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpPost]
    public IActionResult Post([FromBody] ArticleForm form)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ArticleEntity entity = _mapper.Map<ArticleEntity>(form);

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            entity.UserId = userId;

            int articleId = _articleRepository.Create(entity);

            if (articleId <= 0)
            {
                return BadRequest(nameof(articleId));
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(articleId);

            if (articleFromRepo is null)
            {
                return NotFound(NOTEXIST);
            }

            ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

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
            ArticleEntity entity = _mapper.Map<ArticleEntity>(form);

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            entity.UserId = userId;

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

            article.userId = userFromRepo.Id;

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
            {
                return BadRequest(ERROR);
            }

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
            IEnumerable<ArticleCommentModel> articleComments =
            _mapper.Map<IEnumerable<ArticleCommentModel>>(_articleCommentRepository
                .GetArticleComments(id));

            if (_userManager.IsDirection)
            {
                return Ok(articleComments);
            }
            else
            {
                return Ok(articleComments.Where(ac => ac.DeletedAt is null));
            }
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
    public IActionResult PostComment(int id, [FromBody] ArticleCommentForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (!_articleRepository.ArticleExist(id))
            {
                return NotFound(NOTEXIST);
            }

            ArticleCommentEntity commentToCreate = _mapper.Map<ArticleCommentEntity>(form);

            string? userID = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userID))
            {
                return BadRequest();
            }

            commentToCreate.ArticleId = id;

            int commentId = _articleCommentRepository.Create(commentToCreate);

            if (commentId <= 0)
            {
                return BadRequest(nameof(commentId));
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(commentId);

            if (articleCommentFromRepo is null)
            {
                return NotFound(NOTEXIST);
            }

            return Ok();

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
    public IActionResult PutComment(int id, int cId, [FromBody] ArticleCommentForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (!_articleRepository.ArticleExist(id))
            {
                return NotFound(NOTEXIST);
            }

            ArticleCommentEntity articleToCreate = _mapper.Map<ArticleCommentEntity>(form);

            articleToCreate.ArticleId = id;

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            articleToCreate.UserId = userId;

            bool result = _articleCommentRepository.Update(cId, articleToCreate);

            if (!result)
            {
                return BadRequest(nameof(result));
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(cId);

            if (articleCommentFromRepo is null)
            {
                return NotFound(NOTEXIST);
            }

            ArticleCommentModel? comment = _mapper.Map<ArticleCommentModel>(articleCommentFromRepo);

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
            if (!_articleRepository.ArticleExist(id))
            {
                return NotFound(NOTEXIST);
            }

            bool result = _articleCommentRepository.Delete(cId);

            if (!result)
            {
                return BadRequest(ERROR);
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
