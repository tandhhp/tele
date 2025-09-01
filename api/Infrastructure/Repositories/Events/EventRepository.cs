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
                    join u in _context.Users on e.CreatedBy equals u.Id
                    select new
                    {
                        e.Id,
                        e.Name,
                        u.UserName,
                        e.CreatedDate,
                        e.StartDate,
                        CreatedBy = u.Name
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }
}
