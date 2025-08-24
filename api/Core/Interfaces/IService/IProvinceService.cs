using Waffle.Core.Services.Provinces.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface IProvinceService
{
    Task<ListResult<object>> ListAsync(ProvinceFilterOptions filterOptions);
    Task<object?> OptionsAsync(string? keyWords);
}
