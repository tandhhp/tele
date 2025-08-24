using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Departments.Models;
using Waffle.Entities.Users;
using Waffle.Models;

namespace Waffle.Core.Services.Departments;

public class DepartmentService(IDepartmentRepository _departmentRepository, IBranchService _branchService) : IDepartmentService
{
    public async Task<TResult> CreateAsync(CreateDepartmentArgs args)
    {
        var branch = await _branchService.FindAsync(args.BranchId);
        if (branch is null) return TResult.Failed("Không tìm thấy chi nhánh!");
        await _departmentRepository.AddAsync(new Department
        {
            Name = args.Name,
            BranchId = args.BranchId
        });
        return TResult.Success;
    }

    public async Task<TResult> DeleteAsync(int id)
    {
        var department = await _departmentRepository.FindAsync(id);
        if (department is null) return TResult.Failed("Không tìm thấy phòng ban!");
        if (await _departmentRepository.HasTeamAsync(department.Id)) return TResult.Failed("Không thể xóa phòng ban vì có nhóm thuộc phòng ban này!");
        await _departmentRepository.DeleteAsync(department);
        return TResult.Success;
    }

    public Task<Department?> FindAsync(int id) => _departmentRepository.FindAsync(id);

    public Task<ListResult<object>> ListAsync(DepartmentFilterOptions filterOptions) => _departmentRepository.ListAsync(filterOptions);

    public Task<object?> OptionsAsync(SelectOptions selectOptions) => _departmentRepository.OptionsAsync(selectOptions);

    public async Task<TResult> UpdateAsync(UpdateDepartmentArgs args)
    {
        var department = await _departmentRepository.FindAsync(args.Id);
        if (department is null) return TResult.Failed("Không tìm thấy phòng ban!");
        department.Name = args.Name;
        department.BranchId = args.BranchId;
        await _departmentRepository.UpdateAsync(department);
        return TResult.Success;
    }
}
