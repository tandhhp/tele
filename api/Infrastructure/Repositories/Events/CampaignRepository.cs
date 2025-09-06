using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Events;
using Waffle.Core.Services.Events.Models;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories.Events;

public class CampaignRepository(ApplicationDbContext context) : EfRepository<Campaign>(context), ICampaignRepository
{
    public async Task<ListResult<object>> ListAsync(CampaignFilter filter)
    {
        var query = from c in _context.Campaigns
                    select new
                    {
                        c.Id,
                        c.Name,
                        c.CreatedDate,
                        c.CreatedBy,
                        c.ModifiedDate,
                        c.ModifiedBy,
                        c.Code
                    };
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        query = query.OrderByDescending(c => c.CreatedDate);
        return await ListResult<object>.Success(query, filter);
    }

    public async Task<object> OptionsAsync(SelectFilterOptions filterOptions) => await _context.Campaigns
        .Where(x => string.IsNullOrEmpty(filterOptions.KeyWords) || x.Name.ToLower().Contains(filterOptions.KeyWords.ToLower()))
        .Select(x => new
        {
            Label = x.Name,
            Value = x.Id
        }).ToListAsync();
}
