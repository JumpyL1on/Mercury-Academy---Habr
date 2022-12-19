using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.DTOs.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/published-posts")]
    [ApiVersion("2.0")]
    [AllowAnonymous]
    public class PublishedPostControllerV2 : ControllerBase
    {
        private readonly IPublishedPostService _publishedPostService;

        public PublishedPostControllerV2(IPublishedPostService publishedPostService)
        {
            _publishedPostService = publishedPostService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedDTO<PublishedPostDTOV2>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByPageAsyncV2([FromQuery] PaginationQueryParameters parameters)
        {
            return Ok(await _publishedPostService.GetAllByPageAsyncV2(parameters));
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