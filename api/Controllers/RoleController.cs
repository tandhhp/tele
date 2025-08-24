using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Extensions;
using Waffle.Foundations;
using Waffle.Models;

namespace Waffle.Controllers;

public class RoleController : BaseController
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public RoleController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] BasicFilterOptions filterOptions)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user == null) return Unauthorized();
        var isAdmin = User.IsInRole(RoleName.Admin);
        var query = from a in _context.Roles
                    where a.Name != RoleName.CardHolder && a.Name != RoleName.Admin
                    select new
                    {
                        a.Id,
                        a.Name,
                        a.DisplayName,
                        total = (from a1 in _context.UserRoles
                                 join b1 in _context.Users on a1.UserId equals b1.Id
                                 where b1.Status == UserStatus.Working && a1.RoleId == a.Id && (b1.Branch == user.Branch || isAdmin)
                                 select a1.UserId).Count(),
                        leave = (from a1 in _context.UserRoles
                                 join b1 in _context.Users on a1.UserId equals b1.Id
                                 where b1.Status == UserStatus.Leave && a1.RoleId == a.Id && (b1.Branch == user.Branch || isAdmin)
                                 select a1.UserId).Count(),
                        a.Description
                    };
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.Name == RoleName.Sales || x.Name == RoleName.SalesManager);
        }
        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.Name == RoleName.Sales);
        }

        query = query.OrderBy(x => x.Name);

        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpGet("user-options-in-role/{name}")]
    public async Task<IActionResult> GetUserOptionsInRoleAsync([FromRoute] string name)
    {
        var users = await _userManager.GetUsersInRoleAsync(name);
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user == null) return Unauthorized();
        users = users.Where(x => x.Status == UserStatus.Working).Where(x => x.Branch == user.Branch).ToList();
        return Ok(users.Select(x => new
        {
            label = $"{x.Name} - {x.UserName}",
            value = x.Id
        }));
    }

    [HttpPost("delete/{name}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string name)
    {
        var role = await _roleManager.FindByNameAsync(name);
        if (role == null) return NotFound($"Role {name} not found.");
        return Ok(await _roleManager.DeleteAsync(role));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] ApplicationRole role) => Ok(await _roleManager.CreateAsync(role));
}
