using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.Requests;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/draft-posts")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "User, Admin")]
    public class DraftPostController : ControllerBase
    {
        private readonly IDraftPostService _draftPostService;
        private readonly UserManager<User> _userManager;

        public DraftPostController(IDraftPostService postService, UserManager<User> userManager)
        {
            _draftPostService = postService;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedDTO<DraftPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByUserAndPageAsync([FromQuery] PaginationQueryParameters parameters)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return Ok(await _draftPostService.GetAllByUserAndPageAsync(user, parameters));
        }

        [HttpPatch]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EditAsync([FromRoute] int id, [FromBody] EditDraftPostRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            await _draftPostService.EditAsync(id, request, user);

            return Ok();
        }
    }
}