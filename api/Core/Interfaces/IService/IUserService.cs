using Microsoft.AspNetCore.Identity;
using Waffle.Entities;
using Waffle.Models;
using Waffle.Models.Filters;
using Waffle.Models.Users;
using Waffle.Models.ViewModels.Users;

namespace Waffle.Core.Interfaces.IService;

public interface IUserService
{
    Task<IdentityResult> CreateAsync(CreateUserModel model);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);
    Task<CurrentUserViewModel?> GetCurrentUserAsync(Guid id);
    Task<IdentityResult> AddToRoleAsync(AddToRoleModel model);
    Task<dynamic> GetUsersInRoleAsync(string roleName, UserFilterOptions filterOptions);
    Task<IdentityResult> RemoveFromRoleAsync(RemoveFromRoleModel args);
    Task<ListResult<dynamic>> ListContactAsync(ContactFilterOptions filterOptions);
    Task SendEmailToCardHolderAsync(CreateUserModel user);
    Task<ListResult<object>> PointsAsync(UserPointFilterOptions filterOptions);
    Task<TResult> CreateAsync(CreateUserArgs args);
}
