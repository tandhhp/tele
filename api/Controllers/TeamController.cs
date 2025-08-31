using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Teams.Models;
using Waffle.Models.Users.Teams;

namespace Waffle.Controllers;

public class TeamController(ITeamService TeamService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] TeamFilterOptions filterOptions) => Ok(await TeamService.ListAsync(filterOptions));

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTeamArgs args) => Ok(await TeamService.CreateAsync(args));

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTeamArgs args) => Ok(await TeamService.UpdateAsync(args));

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(int id) => Ok(await TeamService.DeleteAsync(id));

    [HttpGet("options")]
    public async Task<IActionResult> OptionsAsync([FromQuery] TeamSelectOptions selectOptions) => Ok(await TeamService.OptionsAsync(selectOptions));

    [HttpGet("users")]
    public async Task<IActionResult> UsersAsync([FromQuery] UserTeamFilterOptions filterOptions) => Ok(await TeamService.UsersAsync(filterOptions));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id) => Ok(await TeamService.GetAsync(id));

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUserAsync([FromBody] AddUserToTeamArgs args) => Ok(await TeamService.AddUserAsync(args));

    [HttpDelete("remove-user")]
    public async Task<IActionResult> RemoveUserAsync([FromBody] RemoveUserFromTeamArgs args) => Ok(await TeamService.RemoveUserAsync(args));
}