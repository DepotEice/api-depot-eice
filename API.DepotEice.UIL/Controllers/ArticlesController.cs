using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        public List<ArticleModel> Articles { get; set; } = new List<ArticleModel>()
        {
            new ArticleModel()
            {
                Title = "Article 1",
                Body = "Body of article 1",
                CreatedAt = DateTime.Now.AddDays(-1),
                Pinned = false
            },
            new ArticleModel()
            {
                Title = "Article 2",
                Body = "Body of article 2",
                CreatedAt = DateTime.Now,
                Pinned = false
            },
        };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Articles);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ArticleForm form)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ArticleForm form)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpGet("{aId}/Comments")]
        public IActionResult GetComments(int aId)
        {
            return Ok();
        }

        [HttpPost("{aId}/Comments")]
        public IActionResult PostComment(int aId, [FromBody] CommentForm form)
        {
            return Ok();
        }

        [HttpPut("{aId}/Comments/{cId}")]
        public IActionResult PutComment(int aId, int cId, [FromBody] CommentForm form)
        {
            return Ok();
        }

        [HttpDelete("{aId}/Comments/{cId}")]
        public IActionResult DeleteComment(int aId, int cId)
        {
            return Ok();
        } 
    }
}
