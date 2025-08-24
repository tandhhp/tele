using Waffle.Core.Services.Teams.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface ITeamService
{
    Task<TResult> CreateAsync(CreateTeamArgs args);
    Task<TResult> DeleteAsync(int id);
    Task<TResult<object?>> GetAsync(int id);
    Task<ListResult<object>> ListAsync(TeamFilterOptions filterOptions);
    Task<object?> OptionsAsync(TeamSelectOptions selectOptions);
    Task<TResult> UpdateAsync(UpdateTeamArgs args);
    Task<ListResult<object>> UsersAsync(UserTeamFilterOptions filterOptions);
}
