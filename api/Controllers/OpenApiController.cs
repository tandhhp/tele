using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waffle.ExternalAPI.Interfaces;
using Waffle.ExternalAPI.Models.WordPress;
using Waffle.Foundations;

namespace Waffle.Controllers;

[AllowAnonymous]
[Route("api/open-api")]
public class OpenApiController : BaseController
{
    private readonly IWordPressService _wordPressService;

    public OpenApiController(IWordPressService wordPressService)
    {
        _wordPressService = wordPressService;
    }

    [HttpGet("wordpress/posts")]
    public async Task<IActionResult> WordPressPostsAsync([FromQuery] OpenWordPressApi args)
    {
        if (string.IsNullOrWhiteSpace(args.Domain)) return BadRequest("Domain is required!");
        return Ok(await _wordPressService.ListPostAsync(args.Domain, args));
    }

    [HttpGet("wordpress/posts/{id}")]
    public async Task<IActionResult> WordPressPostAsync([FromRoute] string id, [FromQuery] string domain)
    {
        if (string.IsNullOrWhiteSpace(domain)) return BadRequest("Domain is required!");
        return Ok(await _wordPressService.GetPostAsync(domain, id));
    }
}
