using Waffle.Core.Services.Districts.Models;
using Waffle.Entities;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository;

public interface IDistrictRepository : IAsyncRepository<District>
{
    Task<ListResult<object>> ListAsync(DistrictFilterOptions filterOptions);
    Task<object> OptionsAsync(DistrictSelectOptions selectOptions);
}
