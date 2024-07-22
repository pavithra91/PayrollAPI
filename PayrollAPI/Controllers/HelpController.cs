using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;

namespace PayrollAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HelpController : ControllerBase
    {
        private readonly IHelp _help;
        public HelpController(IHelp help)
        {
            _help = help;
        }

        [Route("get-categories")]
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            MsgDto _msg = await _help.GetCategories();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("get-articles-id")]
        [HttpGet]
        public async Task<ActionResult> GetArticlesByCategory(int id)
        {
            MsgDto _msg = await _help.GetArticlesByCategory(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("create-article")]
        [HttpPost]
        public async Task<ActionResult> CreateArticle([FromBody] ArticleDto articleDto)
        {
            MsgDto _msg = await _help.CreateArticle(articleDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("update-article")]
        [HttpPut]
        public async Task<ActionResult> UpdateArticle([FromBody] ArticleDto articleDto)
        {
            MsgDto _msg = await _help.UpdateArticle(articleDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }
    }
}
