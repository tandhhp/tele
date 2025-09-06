using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Calls;
using Waffle.Core.Services.Calls.Models;
using Waffle.Data;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories.Calls;

public class CallHistoryRepository(ApplicationDbContext context) : EfRepository<CallHistory>(context), ICallHistoryRepository
{
    public async Task<ListResult<object>> HistoriesAsync(CallHistoryFilterOptions filterOptions)
    {
        var query = from ch in _context.CallHistories
                    join cs in _context.CallStatuses on ch.CallStatusId equals cs.Id
                    select new
                    {
                        ch.Id,
                        ch.CreatedDate,
                        ch.CreatedBy,
                        CallStatus = cs.Name,
                        ch.ContactId,
                        ch.Note,
                        ch.Job,
                        ch.Age,
                        ch.FollowUpDate,
                        ch.TravelHabit
                    };
        if (filterOptions.ContactId.HasValue)
        {
            query = query.Where(x => x.ContactId == filterOptions.ContactId);
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }

    public async Task<TResult<object>> StatisticsAsync()
    {
        var currentYear = DateTime.Now.Year;
        var currentMonth = DateTime.Now.Month;
        var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
        var totalCalls = await _context.CallHistories.CountAsync();
        var currentMonthCalls = await _context.CallHistories.CountAsync(ch => ch.CreatedDate.Year == currentYear && ch.CreatedDate.Month == currentMonth);
        var previousMonthCalls = await _context.CallHistories.CountAsync(ch => ch.CreatedDate.Year == currentYear && ch.CreatedDate.Month == previousMonth);
        var yearlyCalls = await _context.CallHistories.CountAsync(x => x.CreatedDate.Year == currentYear);
        var statistics = new
        {
            TotalCalls = totalCalls,
            CurrentMonthCalls = currentMonthCalls,
            PreviousMonthCalls = previousMonthCalls,
            YearlyCalls = yearlyCalls
        };
        return TResult<object>.Ok(statistics);
    }
}
