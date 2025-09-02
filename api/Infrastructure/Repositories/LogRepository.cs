﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Services.Histories.Models;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.Histories;

namespace Waffle.Infrastructure.Repositories;

public class LogRepository(ApplicationDbContext context) : EfRepository<AppLog>(context), ILogRepository
{
    public async Task<IdentityResult> DeleteAllAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM AppLogs");
        return IdentityResult.Success;
    }

    public async Task<ListResult<HistoryListItem>> ListAsync(HistoryFilterOptions filterOptions)
    {
        var query = from log in _context.AppLogs
                    select new HistoryListItem
                    {
                        Id = log.Id,
                        CreatedDate = log.CreatedDate,
                        Message = log.Message,
                        UserName = log.UserName
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.Message))
        {
            query = query.Where(x => x.Message.ToLower().Contains(filterOptions.Message.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.UserName))
        {
            query = query.Where(x => x.UserName.ToLower().Contains(filterOptions.UserName.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<HistoryListItem>.Success(query, filterOptions);
    }
}
