using Habr.BusinessLogic.Interfaces;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/posts/{postId:int}/comments")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "User, Admin")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;

        public CommentController(ICommentService commentService, UserManager<User> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromRoute] int postId, [FromBody] CreateCommentRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _commentService.CreateAsync(postId, request, user);

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

            await _commentService.DeleteAsync(id, user);

            return Ok();
        }
    }
}