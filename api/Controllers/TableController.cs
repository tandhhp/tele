using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService.Events;
using Waffle.Core.Services.Tables.Models;

namespace Waffle.Controllers;

public class TableController(ITableService _tableService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] TableFilterOptions filterOptions) => Ok(await _tableService.ListAsync(filterOptions));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TableCreateArgs args) => Ok(await _tableService.CreateAsync(args));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] TableUpdateArgs args) => Ok(await _tableService.UpdateAsync(args));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id) => Ok(await _tableService.DeleteAsync(id));
}
