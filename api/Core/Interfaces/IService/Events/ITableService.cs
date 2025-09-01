using Waffle.Core.Services.Tables.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService.Events;

public interface ITableService
{
    Task<TResult> CreateAsync(TableCreateArgs args);
    Task<TResult> DeleteAsync(int id);
    Task<ListResult<object>> ListAsync(TableFilterOptions filterOptions);
    Task<TResult> UpdateAsync(TableUpdateArgs args);
}
