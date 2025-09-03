using Waffle.Core.Services.Calls.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository.Calls;

public interface ICallHistoryRepository : IAsyncRepository<CallHistory>
{
    Task<ListResult<object>> HistoriesAsync(CallHistoryFilterOptions filterOptions);
}
