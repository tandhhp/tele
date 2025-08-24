using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Provinces.Models;
using Waffle.Models;

namespace Waffle.Core.Services.Provinces;

public class ProvinceService(IProvinceRepository _provinceRepository) : IProvinceService
{
    public Task<ListResult<object>> ListAsync(ProvinceFilterOptions filterOptions) => _provinceRepository.ListAsync(filterOptions);

    public Task<object?> OptionsAsync(string? keyWords) => _provinceRepository.OptionsAsync(keyWords);
}
