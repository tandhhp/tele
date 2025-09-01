using Waffle.Core.Services.Tables.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository.Events;

public interface ITableRepository : IAsyncRepository<Table>
{
    Task<ListResult<object>> ListAsync(TableFilterOptions filterOptions);
}
