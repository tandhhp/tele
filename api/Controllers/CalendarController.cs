using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.Calendars;
using Waffle.Models.Filters;

namespace Waffle.Controllers;

public class CalendarController : BaseController
{
    private readonly ApplicationDbContext _context;
    public CalendarController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] CalendarFilterOptions filterOptions)
    {
        var keyIns = await _context.Leads.AsNoTracking()
            .Where(x => x.Branch == filterOptions.Branch)
            .Where(x => x.Status != LeadStatus.Pending)
            .Where(x => x.EventDate != null && x.EventDate.Value.Month == filterOptions.Month && x.EventDate.Value.Year == filterOptions.Year).ToListAsync();

        var plasmas = await (from a in _context.PlasmaCheckIns
                             join b in _context.PlasmaUsers on a.PlasmaUserId equals b.Id
                             where b.Branch == filterOptions.Branch && a.Date.Month == filterOptions.Month && a.Date.Year == filterOptions.Year
                             select a.Date).ToListAsync();

        var calendarData = new List<CalendarListData>();
        foreach (var day in Enumerable.Range(1, DateTime.DaysInMonth(filterOptions.Year, filterOptions.Month)))
        {
            var calendarListData = new CalendarListData { Day = day };
            var items = keyIns.Where(x => x.EventDate.HasValue && x.EventDate.Value.Day == day).ToList();
            foreach (var item in items)
            {
                calendarListData.Items.Add(new CalendarListItem { Content = item.Name });
            }
            calendarListData.EventCount = items.Count;
            calendarListData.PlasmaCount = plasmas.Count(x => x.Day == day);

            calendarData.Add(calendarListData);
        }
        return Ok(new { data = calendarData });
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetEventsAsync([FromQuery] CalendarEventFilterOptions filterOptions)
    {
        var query = from a in _context.Leads
                    join b in _context.Users on a.SalesId equals b.Id into sales
                    from b in sales.DefaultIfEmpty()
                    where a.Branch == filterOptions.Branch && a.EventDate.HasValue && a.EventDate.Value.Date == filterOptions.Date.Date
                    select new
                    {
                        a.Id,
                        a.Name,
                        a.Status,
                        a.EventTime,
                        Seller = b.Name
                    };
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpGet("plasma")]
    public async Task<IActionResult> GetPlasmaAsync([FromQuery] CalendarEventFilterOptions filterOptions)
    {
        var query = from a in _context.PlasmaUsers
                    join b in _context.PlasmaCheckIns on a.Id equals b.PlasmaUserId
                    join c in _context.Users on a.AssistantId equals c.Id into assistants from c in assistants.DefaultIfEmpty()
                    where a.Branch == filterOptions.Branch && b.Date.Date == filterOptions.Date.Date
                    select new
                    {
                        a.Id,
                        a.FullName,
                        b.Time,
                        b.PlasmaType,
                        Assistant = c.Name
                    };
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }
}