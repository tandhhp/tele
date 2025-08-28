using Microsoft.AspNetCore.Identity;
using Waffle.Models;
using Waffle.Models.Histories;

namespace Waffle.Core.Interfaces.IService;

public interface ILogService
{
    Task AddAsync(string message, Guid? catalogId = null);
    Task<IdentityResult> DeleteAllAsync();
    Task<IdentityResult> DeleteAsync(Guid id);
    Task ExceptionAsync(Exception ex);
    Task<ListResult<HistoryListItem>> ListAsync(SearchFilterOptions filterOptions);
}
