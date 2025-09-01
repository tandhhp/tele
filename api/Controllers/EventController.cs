using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService.Events;
using Waffle.Core.Services.Events.Models;
using Waffle.Data;

namespace Waffle.Controllers;

public class EventController(IEventService _eventService) : BaseController
{
    [HttpPost("add-sale-revenue")]
    public async Task<IActionResult> AddSaleRevenueAsync([FromForm] AddSaleRevenue args)
    {
        if (!User.IsInRole(RoleName.Event)) return BadRequest("Bạn không có quyền thực hiện chức năng này");
        var result = await _eventService.AddSaleRevenueAsync(args);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPost("back-to-checkin")]
    public async Task<IActionResult> BackToCheckinAsync([FromBody] BackToCheckin args)
    {
        if (!User.IsInRole(RoleName.Event)) return BadRequest("Bạn không có quyền thực hiện chức năng này");
        var result = await _eventService.BackToCheckinAsync(args);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpGet("list-sale-revenue")]
    public async Task<IActionResult> ListSaleRevenueAsync([FromQuery] SaleRevenueFilterOptions filterOptions) => Ok(await _eventService.ListSaleRevenueAsync(filterOptions));

    [HttpGet("list-key-in-revenue")]
    public async Task<IActionResult> ListKeyInRevenueAsync([FromQuery] SaleRevenueFilterOptions filterOptions) => Ok(await _eventService.ListKeyInRevenueAsync(filterOptions));

    [HttpGet("revenue-history")]
    public async Task<IActionResult> RevenueHistoriesAsync([FromQuery] KeyInRevenueFilterOptions filterOptions) => Ok(await _eventService.RevenueHistoriesAsync(filterOptions));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] EventCreateArgs args) => Ok(await _eventService.CreateAsync(args));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] EventUpdateArgs args) => Ok(await _eventService.UpdateAsync(args));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id) => Ok(await _eventService.DeleteAsync(id));

    [HttpGet("list")]
    public async Task<IActionResult> GetListAsync([FromQuery] EventFilterOptions filterOptions) => Ok(await _eventService.GetListAsync(filterOptions));
}
