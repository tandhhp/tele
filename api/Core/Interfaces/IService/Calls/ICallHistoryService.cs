using Waffle.Core.Services.Calls.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService.Calls;

public interface ICallHistoryService
{
    Task<ListResult<object>> HistoriesAsync(CallHistoryFilterOptions filterOptions);
    Task<TResult> CompleteAsync(CallCompleteArgs args);
    Task<TResult<object>> StatisticsAsync();
}
