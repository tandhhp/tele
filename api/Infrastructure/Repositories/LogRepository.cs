using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.ViewModels.Logs;

namespace Waffle.Infrastructure.Repositories;

public class LogRepository : EfRepository<AppLog>, ILogRepository
{
    public LogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IdentityResult> DeleteAllAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM AppLogs");
        return IdentityResult.Success;
    }

    public async Task<ListResult<AppLogListItem>> ListAsync(SearchFilterOptions filterOptions)
    {
        var query = from log in _context.AppLogs
                    select new AppLogListItem
                    {
                        CatalogId = log.Id,
                        Id = log.Id,
                        CreatedDate = log.CreatedDate,
                        Message = log.Message
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.SearchTerm))
        {
            query = query.Where(x => x.Message.ToLower().Contains(filterOptions.SearchTerm.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<AppLogListItem>.Success(query, filterOptions);
    }
}
