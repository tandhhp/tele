using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Core.Interfaces.IService;
using Waffle.Foundations;
using Waffle.Models;

namespace Waffle.Controllers;

public class LogController : BaseController
{
    private readonly ILogService _logService;

    public LogController(ILogService appLogService)
    {
        _logService = appLogService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] SearchFilterOptions filterOptions) => Ok(await _logService.ListAsync(filterOptions));

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) => Ok(await _logService.DeleteAsync(id));

    [HttpPost("delete/all")]
    public async Task<IActionResult> DeleteAllAsync() => Ok(await _logService.DeleteAllAsync());
}
