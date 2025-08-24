using Microsoft.AspNetCore.Identity;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.ViewModels.Logs;

namespace Waffle.Core.Interfaces.IRepository;

public interface ILogRepository : IAsyncRepository<AppLog>
{
    Task<IdentityResult> DeleteAllAsync();
    Task<ListResult<AppLogListItem>> ListAsync(SearchFilterOptions filterOptions);
}
