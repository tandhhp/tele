using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Constants;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces.IService;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Entities.Plasma;
using Waffle.Foundations;
using Waffle.Models.Filters;

namespace Waffle.Controllers;

public class PlasmaController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;
    public PlasmaController(ApplicationDbContext context, ILogService logService)
    {
        _context = context;
        _logService = logService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] PlasmaFilterOptions filterOptions)
    {
        var query = from a in _context.PlasmaUsers
                    join u in _context.Users on a.AssistantId equals u.Id
                    join b in _context.UserRoles on u.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.Plasma && u.Status == UserStatus.Working
                    select new PlasmaUser
                    {
                        Id = a.Id,
                        FullName = a.FullName,
                        IdentityNumber = a.IdentityNumber,
                        Gender = a.Gender,
                        PhoneNumber = a.PhoneNumber,
                        Email = a.Email,
                        Address = a.Address,
                        DateOfBirth = a.DateOfBirth,
                        CreatedDate = a.CreatedDate,
                        ModifiedDate = a.ModifiedDate,
                        TotalDays = a.TotalDays,
                        AssistantId = a.AssistantId,
                        Branch = a.Branch,
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
            Total = await query.CountAsync()
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] PlasmaUser plasmaUser)
    {
        try
        {
            if (!User.IsInRole(RoleName.Plasma))
                return BadRequest("Không có quyền thực hiện chức năng này");
            if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(plasmaUser.PhoneNumber))
                return BadRequest("Số điện thoại không hợp lệ");
            bool exists;
            if (string.IsNullOrWhiteSpace(plasmaUser.IdentityNumber))
            {
                exists = await _context.PlasmaUsers.AnyAsync(x => x.PhoneNumber == plasmaUser.PhoneNumber);
                if (exists) return BadRequest("Số điện thoại đã tồn tại");
            }
            else
            {
                if (plasmaUser.IdentityNumber.Length != 12) return BadRequest("Số CCCD không hợp lệ");
                exists = await _context.PlasmaUsers.AnyAsync(x => x.IdentityNumber == plasmaUser.IdentityNumber);
                if (exists) return BadRequest("CCCD đã tồn tại");
            }
            plasmaUser.CreatedDate = DateTime.Now;
            await _context.PlasmaUsers.AddAsync(plasmaUser);
            await _context.SaveChangesAsync();
            return Ok(plasmaUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }


    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] PlasmaUser args)
    {
        if (!User.IsInRole(RoleName.Plasma) && !User.IsInRole(RoleName.Admin)) return BadRequest("Không có quyền thực hiện chức năng này");
        var data = await _context.PlasmaUsers.FindAsync(args.Id);
        if (data is null) return BadRequest("Không tìm thấy dữ liệu");
        if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(args.PhoneNumber)) return BadRequest("Số điện thoại không hợp lệ");
        bool exists;
        if (string.IsNullOrWhiteSpace(args.IdentityNumber))
        {
            exists = await _context.PlasmaUsers.AnyAsync(x => x.PhoneNumber == args.PhoneNumber && x.Id != args.Id);
            if (exists) return BadRequest("Số điện thoại đã tồn tại");
        }
        else
        {
            if (args.IdentityNumber.Length != 12) return BadRequest("Số CCCD không hợp lệ");
            exists = await _context.PlasmaUsers.AnyAsync(x => x.IdentityNumber == args.IdentityNumber && x.Id != args.Id);
            if (exists) return BadRequest("CMND/CCCD đã tồn tại");
        }
        data.ModifiedDate = DateTime.Now;
        data.Address = args.Address;
        data.DateOfBirth = args.DateOfBirth;
        data.Email = args.Email;
        data.Gender = args.Gender;
        data.FullName = args.FullName;
        data.PhoneNumber = args.PhoneNumber;
        data.AssistantId = args.AssistantId;
        data.TotalDays = args.TotalDays;
        await _logService.AddAsync($"Cập nhật thông tin người plasma {data.FullName}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        if (!User.IsInRole(RoleName.Plasma)) return BadRequest("Không có quyền thực hiện chức năng này");
        var data = await _context.PlasmaUsers.FindAsync(id);
        if (data is null) return BadRequest("Không tìm thấy dữ liệu");
        _context.PlasmaUsers.Remove(data);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("technician-options")]
    public async Task<IActionResult> TechnicianOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.Plasma && a.Status == UserStatus.Working
                    select new
                    {
                        Value = a.Id,
                        Label = a.Name,
                        Branch = a.Branch,
                    };
        return Ok(await query.ToListAsync());
    }

    [HttpGet("count")]
    public async Task<IActionResult> CountAsync() => Ok(new { data = await _context.PlasmaUsers.CountAsync() });
}
