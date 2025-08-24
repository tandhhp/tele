using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Events.Models;
using Waffle.Data;
using Waffle.Foundations;
using Waffle.Models;

namespace Waffle.Controllers;

public class EventController : BaseController
{
    private readonly IEventService _eventService;
    private readonly ApplicationDbContext _context;
    public EventController(IEventService eventService, ApplicationDbContext context)
    {
        _eventService = eventService;
        _context = context;
    }

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
}
