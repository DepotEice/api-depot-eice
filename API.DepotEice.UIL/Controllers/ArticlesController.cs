using API.DepotEice.BLL.IServices;
using API.DepotEice.UIL.Mapper;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("IsConnected")]
    public class ArticlesController : ControllerBase
    {
        private const string NOTEXIST = "The selected item does not exist ! Please try again or with another one.";
        private const string ERROR = "Something went wrong ! Please contact the administrator ...";

        private readonly IArticleService _articleService;
        private readonly IArticleCommentService _articleCommentService;

        public ArticlesController(IArticleService articleService, IArticleCommentService articleCommentService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<ArticleModel> articles = _articleService.GetAll().Select(x => x.ToUil());
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
                ArticleModel? article = _articleService.GetByKey(id)?.ToUil();
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
                ArticleModel? result = _articleService.Create(GetUserId(), form.ToBll())?.ToUil();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ArticleForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                ArticleModel? result = _articleService.Update(id, GetUserId(), form.ToBll())?.ToUil();
                return Ok(result);
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
                bool result = _articleService.Delete(id);

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
                IEnumerable<CommentModel> comments = _articleCommentService.GetAll(id).Select(x => x.ToUil());
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

                CommentModel? result = _articleCommentService.Create(id, GetUserId(), form.ToBll())?.ToUil();
                return Ok(result);

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

                CommentModel? result = _articleCommentService.Update(id, GetUserId(), form.ToBll())?.ToUil();
                return Ok(result);

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

                bool result = _articleCommentService.Delete(cId);

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
            ArticleModel? article = _articleService.GetByKey(id)?.ToUil();

            if (article == null)
                return false;
            else
                return true;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.Sid).Value;
        }
    }
}
