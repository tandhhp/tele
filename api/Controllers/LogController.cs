using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Histories.Models;
using Waffle.Models;

namespace Waffle.Controllers;

public class LogController(ILogService _logService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] HistoryFilterOptions filterOptions) => Ok(await _logService.ListAsync(filterOptions));

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) => Ok(await _logService.DeleteAsync(id));

    [HttpPost("delete/all")]
    public async Task<IActionResult> DeleteAllAsync() => Ok(await _logService.DeleteAllAsync());
}
