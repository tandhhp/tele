using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Teams.Models;
using Waffle.Entities;
using Waffle.Entities.Users;
using Waffle.Models;
using Waffle.Models.Users.Teams;

namespace Waffle.Core.Services.Teams;

public class TeamService(ITeamRepository _teamRepository, IDepartmentService _departmentService, UserManager<ApplicationUser> _userManager) : ITeamService
{
    public async Task<TResult> AddUserAsync(AddUserToTeamArgs args)
    {
        var user = await _userManager.FindByIdAsync(args.UserId.ToString());
        if (user is null) return TResult.Failed("Không tìm thấy người dùng!");
        var team = await _teamRepository.FindAsync(args.TeamId);
        if (team is null) return TResult.Failed("Không tìm thấy nhóm!");
        user.TeamId = team.Id;
        await _userManager.UpdateAsync(user);
        return TResult.Success;
    }

    public async Task<TResult> CreateAsync(CreateTeamArgs args)
    {
        var department = await _departmentService.FindAsync(args.DepartmentId);
        if (department is null) return TResult.Failed("Không tìm thấy phòng ban!");
        if (string.IsNullOrWhiteSpace(args.Name)) return TResult.Failed("Tên nhóm không được để trống!");
        if (await _teamRepository.ExistsAsync(args.Name, args.DepartmentId)) return TResult.Failed("Tên nhóm đã tồn tại trong phòng ban này!");
        var team = new Team
        {
            Name = args.Name,
            DepartmentId = args.DepartmentId
        };
        await _teamRepository.AddAsync(team);
        return TResult.Success;
    }

    public async Task<TResult> DeleteAsync(int id)
    {
        var team = await _teamRepository.FindAsync(id);
        if (team is null) return TResult.Failed("Không tìm thấy nhóm!");
        if (await _userManager.Users.AnyAsync(x => x.TeamId == team.Id))
        {
            return TResult.Failed("Không thể xóa nhóm vì có người dùng đang thuộc nhóm này!");
        }
        await _teamRepository.DeleteAsync(team);
        return TResult.Success;
    }

    public async Task<TResult<object?>> GetAsync(int id)
    {
        var team = await _teamRepository.FindAsync(id);
        if (team is null) return TResult<object?>.Failed("Không tìm thấy nhóm!");
        return TResult<object?>.Ok(new
        {
            team.Id,
            team.Name,
            team.DepartmentId
        });
    }

    public Task<ListResult<object>> ListAsync(TeamFilterOptions filterOptions) => _teamRepository.ListAsync(filterOptions);

    public Task<object?> OptionsAsync(TeamSelectOptions selectOptions) => _teamRepository.OptionsAsync(selectOptions);

    public async Task<TResult> RemoveUserAsync(RemoveUserFromTeamArgs args)
    {
        var user = await _userManager.FindByIdAsync(args.UserId.ToString());
        if (user is null) return TResult.Failed("Không tìm thấy người dùng!");
        var team = await _teamRepository.FindAsync(args.TeamId);
        if (team is null) return TResult.Failed("Không tìm thấy nhóm!");
        if (user.TeamId != team.Id) return TResult.Failed("Người dùng không thuộc nhóm này!");
        user.TeamId = null;
        await _userManager.UpdateAsync(user);
        return TResult.Success;
    }

    public async Task<TResult> UpdateAsync(UpdateTeamArgs args)
    {
        var team = await _teamRepository.FindAsync(args.Id);
        if (team is null) return TResult.Failed("Không tìm thấy nhóm!");
        var department = await _departmentService.FindAsync(args.DepartmentId);
        if (department is null) return TResult.Failed("Không tìm thấy phòng ban!");
        team.Name = args.Name;
        team.DepartmentId = args.DepartmentId;
        await _teamRepository.UpdateAsync(team);
        return TResult.Success;
    }

    public Task<ListResult<object>> UsersAsync(UserTeamFilterOptions filterOptions) => _teamRepository.UsersAsync(filterOptions);
}
