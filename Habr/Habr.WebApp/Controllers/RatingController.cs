using Habr.BusinessLogic.Interfaces;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/posts/{postId:int}/ratings")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "User, Admin")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly UserManager<User> _userManager;

        public RatingController(IRatingService ratingService, UserManager<User> userManager)
        {
            _ratingService = ratingService;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromRoute] int postId, [FromBody] CreateRatingRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _ratingService.CreateAsync(postId, request, user);

            return Ok();
        }
    }
}