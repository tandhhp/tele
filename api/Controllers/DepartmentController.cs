using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Departments.Models;
using Waffle.Foundations;
using Waffle.Models;

namespace Waffle.Controllers;

public class DepartmentController(IDepartmentService _departmentService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] DepartmentFilterOptions filterOptions) => Ok(await _departmentService.ListAsync(filterOptions));

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDepartmentArgs args) => Ok(await _departmentService.CreateAsync(args));

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateDepartmentArgs args) => Ok(await _departmentService.UpdateAsync(args));

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(int id) => Ok(await _departmentService.DeleteAsync(id));

    [HttpGet("options")]
    public async Task<IActionResult> OptionsAsync([FromQuery] SelectOptions selectOptions) => Ok(await _departmentService.OptionsAsync(selectOptions));
}
