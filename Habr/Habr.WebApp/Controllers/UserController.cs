using Habr.BusinessLogic.Interfaces;
using Habr.Common.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("current/email")]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetCurrentUserEmail()
        {
            var claims = HttpContext.User.Claims;

            return Ok(claims.First(claim => claim.Type == ClaimTypes.Email).Value);
        }

        [HttpPost]
        [Route("sign-up")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            return Ok(await _userService.RegisterAsync(request));
        }

        [HttpPost]
        [Route("sign-in")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request)
        {
            return Ok(await _userService.LoginAsync(request));
        }

        [HttpPost]
        [Route("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            return Ok(await _userService.RefreshTokenAsync(request));
        }
    }
}