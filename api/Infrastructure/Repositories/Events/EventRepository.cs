using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Events;
using Waffle.Core.Services.Events.Models;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories.Events;

public class EventRepository(ApplicationDbContext context) : EfRepository<Event>(context), IEventRepository
{
    public async Task<ListResult<object>> GetListAsync(EventFilterOptions filterOptions)
    {
        var query = from e in _context.Events
                    join c in _context.Campaigns on e.CampaignId equals c.Id into ec from c in ec.DefaultIfEmpty()
                    select new
                    {
                        e.Id,
                        e.Name,
                        e.CreatedDate,
                        e.StartDate,
                        CampaignName = c.Name,
                        e.CampaignId,
                        e.Status
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }
}
