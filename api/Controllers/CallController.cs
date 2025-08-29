using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService.Tele;
using Waffle.Models;

namespace Waffle.Controllers;

public class CallController(ICallStatusService _callStatusService) : BaseController
{
    [HttpGet("status/options")]
    public async Task<IActionResult> StatusOptionsAsync([FromQuery] SelectOptions? options) => Ok(await _callStatusService.OptionsAsync(options));
}
