using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.KeyIn.Models;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Foundations;

namespace Waffle.Controllers;

public class LeadController : BaseController
{
    private readonly IKeyInService _keyInService;
    public LeadController(IKeyInService keyInService)
    {
        _keyInService = keyInService;
    }

    [HttpGet("status-options")]
    public IActionResult GetStatusOptions()
    {
        var result = Enum.GetValues<LeadStatus>().Select(x => new
        {
            Value = x,
            Label = EnumHelper.GetDisplayName(x)
        });
        return Ok(result);
    }

    [HttpPost("update-branch")]
    public async Task<IActionResult> UpdateBranchAsync([FromBody] UpdateBranchArgs args) => Ok(await _keyInService.UpdateBranchAsync(args));
}
