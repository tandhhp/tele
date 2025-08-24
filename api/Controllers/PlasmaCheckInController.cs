using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Core.Interfaces.IService;
using Waffle.Data;
using Waffle.Entities.Plasma;
using Waffle.Entities;
using Waffle.Foundations;
using Waffle.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Waffle.Extensions;

namespace Waffle.Controllers;

public class PlasmaCheckInController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;
    public PlasmaCheckInController(ApplicationDbContext context, ILogService logService)
    {
        _context = context;
        _logService = logService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] PlasmaFilterOptions filterOptions)
    {
        var query = from pc in _context.PlasmaCheckIns
                    join pu in _context.PlasmaUsers on pc.PlasmaUserId equals pu.Id
                    join u in _context.Users on pu.AssistantId equals u.Id
                    join b in _context.UserRoles on u.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.Plasma && u.Status == UserStatus.Working
                    select new PlasmaUserViewModel
                    {
                        Id = pc.Id,
                        PlasmaUserId = pc.PlasmaUserId,
                        Date = pc.Date,
                        Time = pc.Time,
                        PlasmaType = pc.PlasmaType,
                        FullName = pu.FullName,
                        IdentityNumber = pu.IdentityNumber,
                        Gender = pu.Gender,
                        PhoneNumber = pu.PhoneNumber,
                        Email = pu.Email,
                        Address = pu.Address,
                        DateOfBirth = pu.DateOfBirth,
                        CreatedDate = pu.CreatedDate,
                        ModifiedDate = pu.ModifiedDate,
                        TotalDays = pu.TotalDays,
                        AssistantId = pu.AssistantId,
                        SupporterName = u.Name,
                        Branch = pu.Branch,
                        Status = pc.Status
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.FullName))
        {
            query = query.Where(x => x.FullName.ToLower().Contains(filterOptions.FullName.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Email))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(filterOptions.Email.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber.ToLower()));
        }

        return Ok(new
        {
            Data = await query.OrderByDescending(x => x.CreatedDate).Skip((filterOptions.Current - 1) * filterOptions.PageSize).ToListAsync(),
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] PlasmaCheckIn plasmaCheckIn)
    {
        try
        {
            if (!User.IsInRole(RoleName.Plasma)) return BadRequest("Không có quyền thực hiện chức năng này");
            plasmaCheckIn.CreatedDate = DateTime.Now;
            plasmaCheckIn.CreatedBy = User.GetId();
            plasmaCheckIn.Status = PlasmaCheckInStatus.Scheduled;
            await _context.PlasmaCheckIns.AddAsync(plasmaCheckIn);
            await _context.SaveChangesAsync();
            return Ok(plasmaCheckIn);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] PlasmaCheckIn args)
    {
        if (!User.IsInRole(RoleName.Plasma)) return BadRequest("Không có quyền thực hiện chức năng này");
        var data = await _context.PlasmaCheckIns.FindAsync(args.Id);
        if (data is null) return BadRequest("Không tìm thấy dữ liệu");
        data.ModifiedDate = DateTime.Now;
        data.ModifiedBy = User.GetId();
        data.PlasmaUserId = args.PlasmaUserId;
        data.Date = args.Date;
        data.Time = args.Time;
        data.PlasmaType = args.PlasmaType;
        var plasmaUserData = await _context.PlasmaUsers.FindAsync(data.PlasmaUserId);
        if (plasmaUserData is null) return BadRequest("Không tìm thấy dữ liệu người plasma");
        await _logService.AddAsync($"Cập nhật thông tin checkIn {plasmaUserData.FullName}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        if (!User.IsInRole(RoleName.Plasma)) return BadRequest("Không có quyền thực hiện chức năng này");
        var data = await _context.PlasmaCheckIns.FindAsync(id);
        if (data is null) return BadRequest("Không tìm thấy dữ liệu");
        _context.PlasmaCheckIns.Remove(data);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("plasmaUser")]
    public async Task<IActionResult> PlasmaUser()
    {
        var dataPlasmaUser = from pu in _context.PlasmaUsers
                             join a in _context.Users on pu.AssistantId equals a.Id
                             join b in _context.UserRoles on a.Id equals b.UserId
                             join c in _context.Roles on b.RoleId equals c.Id
                             where c.Name == RoleName.Plasma && a.Status == UserStatus.Working
                             select new
                             {
                                 Value = pu.Id,
                                 Label = pu.FullName,
                                 Gender = pu.Gender == true ? "Nam" : "Nữ",
                                 pu.PhoneNumber,
                                 pu.TotalDays,
                                 SupporterName = a.Name,
                                 Branch = pu.Branch == 0 ? "Miền Nam" : "Miền Bắc",
                             };
        return Ok(await dataPlasmaUser.ToListAsync());
    }

    [HttpPost("change-status/{id}")]
    public async Task<IActionResult> ChangeStatusAsync([FromRoute] Guid id, [FromBody] PlasmaCheckInStatus status)
    {
        if (!User.IsInRole(RoleName.Plasma)) return BadRequest("Không có quyền thực hiện chức năng này");
        var data = await _context.PlasmaCheckIns.FindAsync(id);
        if (data is null) return BadRequest("Không tìm thấy dữ liệu");
        data.Status = status;
        data.ModifiedDate = DateTime.Now;
        data.ModifiedBy = User.GetId();
        var plasmaUserData = await _context.PlasmaUsers.FindAsync(data.PlasmaUserId);
        if (plasmaUserData is null) return BadRequest("Không tìm thấy dữ liệu người plasma");
        if (data.Status == PlasmaCheckInStatus.CheckedIn)
        {
            plasmaUserData.TotalDays--;
        }
        await _logService.AddAsync($"Cập nhật trạng thái checkIn {plasmaUserData.FullName} thành {status}");
        await _context.SaveChangesAsync();
        return Ok();
    }
}

