using Habr.BusinessLogic.Interfaces;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/posts")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "User, Admin")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPublishedPostService _publishedPostService;
        private readonly IDraftPostService _draftPostService;
        private readonly UserManager<User> _userManager;

        public PostController(
            IPostService postService,
            IPublishedPostService publishedPostService,
            IDraftPostService draftPostService,
            UserManager<User> userManager)
        {
            _postService = postService;
            _publishedPostService = publishedPostService;
            _draftPostService = draftPostService;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePostRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _postService.CreateAsync(request, user);

            return Ok();
        }

        [HttpPut]
        [Route("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeStatusAsync([FromRoute] int id, [FromBody] bool moveToDraft)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (moveToDraft)
            {
                await _publishedPostService.MoveToDraftsAsync(id, user);
            }
            else
            {
                await _draftPostService.PublishAsync(id, user);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _postService.DeleteAsync(id, user);

            return Ok();
        }
    }
}