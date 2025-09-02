using Microsoft.AspNetCore.Identity;
using Waffle.Core.Services.Histories.Models;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.Histories;

namespace Waffle.Core.Interfaces.IRepository;

public interface ILogRepository : IAsyncRepository<AppLog>
{
    Task<IdentityResult> DeleteAllAsync();
    Task<ListResult<HistoryListItem>> ListAsync(HistoryFilterOptions filterOptions);
}
