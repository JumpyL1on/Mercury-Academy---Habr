using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/published-posts")]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class PublishedPostController : ControllerBase
    {
        private readonly IPublishedPostService _publishedPostService;

        public PublishedPostController(IPublishedPostService publishedPostService)
        {
            _publishedPostService = publishedPostService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedDTO<PublishedPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByPageAsync([FromQuery] PaginationQueryParameters parameters)
        {
            return Ok(await _publishedPostService.GetAllByPageAsync(parameters));
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(PublishedPostDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return Ok(await _publishedPostService.GetByIdAsync(id));
        }
    }
}