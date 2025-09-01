using Waffle.Core.Services.Events.Models;
using Waffle.Entities;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository.Events;

public interface IEventRepository : IAsyncRepository<Event>
{
    Task<ListResult<object>> GetListAsync(EventFilterOptions filterOptions);
}
