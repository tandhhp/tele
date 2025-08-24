using Waffle.Core.Services.Districts.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface IDistrictService
{
    Task<TResult> CreateAsync(CreateDistrictArgs args);
    Task<TResult> DeleteAsync(int id);
    Task<ListResult<object>> ListAsync(DistrictFilterOptions filterOptions);
    Task<object> OptionsAsync(DistrictSelectOptions selectOptions);
    Task<TResult> UpdateAsync(UpdateDistrictArgs args);
}
