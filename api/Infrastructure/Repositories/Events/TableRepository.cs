using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Events;
using Waffle.Core.Services.Tables.Models;
using Waffle.Data;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories.Events;

public class TableRepository(ApplicationDbContext context) : EfRepository<Table>(context), ITableRepository
{
    public async Task<ListResult<object>> ListAsync(TableFilterOptions filterOptions)
    {
        var query = from a in _context.Tables
                    join b in _context.Rooms on a.RoomId equals b.Id
                    select new
                    {
                        a.Id,
                        a.Name,
                        a.Status,
                        a.SortOrder,
                        a.CreatedDate,
                        RoomName = b.Name,
                        a.RoomId,
                        b.BranchId
                    };
        if (filterOptions.RoomId.HasValue)
        {
            query = query.Where(x => x.RoomId == filterOptions.RoomId);
        }
        if (filterOptions.BranchId != null)
        {
            query = query.Where(x => x.BranchId == filterOptions.BranchId);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }
}
