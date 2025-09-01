using Microsoft.AspNetCore.Identity;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.Histories;

namespace Waffle.Core.Services;

public class LogService(IWebHostEnvironment _webHostEnvironment, ICurrentUser _currentUser, ILogRepository _logRepository) : ILogService
{
    public async Task AddAsync(string message, Guid? catalogId)
    {
        await _logRepository.AddAsync(new AppLog
        {
            Message = message,
            CatalogId = catalogId ?? Guid.Empty,
            CreatedDate = DateTime.Now,
            UserId = _currentUser.GetId()
        });
    }

    public Task<IdentityResult> DeleteAllAsync() => _logRepository.DeleteAllAsync();

    public async Task<IdentityResult> DeleteAsync(Guid id)
    {
        var log = await _logRepository.FindAsync(id);
        if (log is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "error.dataNotFound",
                Description = "Log not found!"
            });
        }
        await _logRepository.DeleteAsync(log);
        return IdentityResult.Success;
    }

    public async Task ExceptionAsync(Exception ex)
    {
        if (_webHostEnvironment.IsDevelopment()) return;
        await _logRepository.AddAsync(new AppLog
        {
            Message = ex.ToString(),
            CreatedDate = DateTime.Now,
            UserId = _currentUser.GetId(),
            CatalogId = Guid.Empty
        });
    }

    public Task<ListResult<HistoryListItem>> ListAsync(SearchFilterOptions filterOptions) => _logRepository.ListAsync(filterOptions);
}
