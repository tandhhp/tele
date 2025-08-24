using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Provinces.Models;
using Waffle.Foundations;

namespace Waffle.Controllers;

public class ProvinceController(IProvinceService _provinceService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] ProvinceFilterOptions filterOptions) => Ok(await _provinceService.ListAsync(filterOptions));

    [HttpGet("options")]
    public async Task<IActionResult> OptionsAsync([FromQuery] string? keyWords) => Ok(await _provinceService.OptionsAsync(keyWords));
}
