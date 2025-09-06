using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService.Calls;
using Waffle.Core.Services.Calls.Models;
using Waffle.Models;

namespace Waffle.Controllers;

public class CallController(ICallStatusService _callStatusService, ICallHistoryService _callHistoryService) : BaseController
{
    [HttpGet("status/options")]
    public async Task<IActionResult> StatusOptionsAsync([FromQuery] SelectOptions options) => Ok(await _callStatusService.OptionsAsync(options));

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteAsync([FromBody] CallCompleteArgs args) => Ok(await _callHistoryService.CompleteAsync(args));

    [HttpGet("histories")]
    public async Task<IActionResult> HistoriesAsync([FromQuery] CallHistoryFilterOptions filterOptions) => Ok(await _callHistoryService.HistoriesAsync(filterOptions));

    [HttpGet("statistics")]
    public async Task<IActionResult> StatisticsAsync() => Ok(await _callHistoryService.StatisticsAsync());
}
