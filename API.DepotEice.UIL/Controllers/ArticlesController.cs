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
    /// <param name="userManager"></param>
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
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int? skip = null, int? top = null)
    {
        try
        {
            IEnumerable<ArticleEntity> articlesFromRepo = _articleRepository.GetAll();

            if (articlesFromRepo is null)
            {
                return NotFound();
            }

            if (skip.HasValue)
            {
                articlesFromRepo = articlesFromRepo.Skip(skip.Value);
            }

            if (top.HasValue)
            {
                articlesFromRepo = articlesFromRepo.Take(top.Value);
            }

            IEnumerable<ArticleModel> articles = _mapper.Map<IEnumerable<ArticleModel>>(articlesFromRepo);

            return Ok(articles);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Get)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get all articles, please contact the administrator");
#endif
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

            article.UserId = userFromRepo.Id;

            return Ok(article);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Get)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to get the article with ID \"{id}\", please contact the administrator");
#endif
        }
    }

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
        if (form is null)
        {
            return BadRequest("The form is null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            ArticleEntity entity = _mapper.Map<ArticleEntity>(form);

            entity.UserId = userId;

            int articleId = _articleRepository.Create(entity);

            if (articleId <= 0)
            {
                return BadRequest("The article creation failed");
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(articleId);

            if (articleFromRepo is null)
            {
                return NotFound("The newly created article cannot be found");
            }

            ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

            return Ok(article);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Post)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to create an article, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="form"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ArticleForm form)
    {
        if (form is null)
        {
            return BadRequest("The form is null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            ArticleEntity entity = _mapper.Map<ArticleEntity>(form);

            entity.Id = id;

            entity.UserId = userId;

            bool result = _articleRepository.Update(id, entity);

            if (!result)
            {
                return BadRequest("The update of the article failed");
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(entity.Id);

            if (articleFromRepo is null)
            {
                return NotFound("The updated article cannot be found");
            }

            ArticleModel article = _mapper.Map<ArticleModel>(articleFromRepo);

            return Ok(article);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Put)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to update the article with ID \"{id}\", please contact the administrator");
#endif
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
        if (id <= 0)
        {
            return BadRequest("The id is invalid");
        }

        try
        {
            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound($"The article with ID \"{id}\" doesn't exist");
            }

            if (articleFromRepo.DeletedAt is not null)
            {
                return BadRequest($"The article with ID \"{id}\" is already deleted");
            }

            bool result = _articleRepository.Delete(id);

            if (!result)
            {
                return BadRequest("The deletion failed");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Delete)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to delete the article with ID \"{id}\", please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Restore article
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpPut("Restore/{id}")]
    public IActionResult Restore(int id)
    {
        if (id <= 0)
        {
            return BadRequest("The id is invalid");
        }

        try
        {
            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound("There is not article with the provided id");
            }

            articleFromRepo.DeletedAt = null;

            if (!_articleRepository.Restore(id))
            {
                return BadRequest("Restoring the article failed");
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
    /// <returns></returns>
    [HttpGet("{id}/Comments")]
    [AllowAnonymous]
    public IActionResult GetComments(int id)
    {
        if (id <= 0)
        {
            return BadRequest("The id is invalid");
        }

        try
        {
            IEnumerable<ArticleCommentModel> articleComments =
                _mapper.Map<IEnumerable<ArticleCommentModel>>(_articleCommentRepository.GetArticleComments(id));

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
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetComments)}\" :\n" +
                 $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to get article comments, please contact the administrator");
#endif
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
        if (id <= 0)
        {
            return BadRequest("The id is invalid");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound("There is no article with this id");
            }

            if (articleFromRepo.DeletedAt is not null)
            {
                return BadRequest("The article is deleted");
            }

            ArticleCommentEntity commentToCreate = _mapper.Map<ArticleCommentEntity>(form);

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            commentToCreate.ArticleId = id;
            commentToCreate.UserId = userId;

            int commentId = _articleCommentRepository.Create(commentToCreate);

            if (commentId <= 0)
            {
                return BadRequest("The article comment couldn't be created");
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(commentId);

            if (articleCommentFromRepo is null)
            {
                return NotFound("The newly created article comment could not be found");
            }

            ArticleCommentModel articleComment = _mapper.Map<ArticleCommentModel>(articleCommentFromRepo);

            return Ok(articleComment);

        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(PostComment)}\" :\n" +
                 $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to create an article comment, please contact the administrator");
#endif
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
        if (id <= 0)
        {
            return BadRequest("The article id is invalid");
        }

        if (cId <= 0)
        {
            return BadRequest("The comment id is invalid");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound("There is no article with this id");
            }

            if (articleFromRepo.DeletedAt is not null)
            {
                return BadRequest("The article is deleted");
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(cId);

            if (articleCommentFromRepo is null)
            {
                return NotFound("There is no comment with this id");
            }

            if (articleCommentFromRepo.DeletedAt is not null)
            {
                return BadRequest("The comment is deleted");
            }

            _mapper.Map(form, articleCommentFromRepo);

            bool result = _articleCommentRepository.Update(cId, articleCommentFromRepo);

            if (!result)
            {
                return BadRequest("The update failed");
            }

            articleCommentFromRepo = _articleCommentRepository.GetByKey(cId);

            if (articleCommentFromRepo is null)
            {
                return NotFound("The udpated article comment cannot be found");
            }

            ArticleCommentModel comment = _mapper.Map<ArticleCommentModel>(articleCommentFromRepo);

            return Ok(comment);

        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(PutComment)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to update the article comment with ID \"{cId}\", please contact the administrator");
#endif
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
        if (id <= 0)
        {
            return BadRequest("The article id is invalid");
        }

        if (cId <= 0)
        {
            return BadRequest("The comment id is invalid");
        }

        try
        {
            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                return NotFound("There is no article with this id");
            }

            if (articleFromRepo.DeletedAt is not null)
            {
                return BadRequest("The article is deleted");
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(cId);

            if (articleCommentFromRepo is null)
            {
                return NotFound("There is no comment with this id");
            }

            if (articleCommentFromRepo.DeletedAt is not null)
            {
                return BadRequest("The comment is deleted");
            }

            bool result = _articleCommentRepository.Delete(cId);

            if (!result)
            {
                return BadRequest("The deletion of the article comment failed");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(DeleteComment)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest($"An error occurred while trying to delete the article comment with ID \"{cId}\", please contact the administrator");
#endif
        }
    }
}
