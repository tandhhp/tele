using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Constants;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Debts;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Foundations;

namespace Waffle.Controllers;

public class DebtController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ILogService _logService;
    public DebtController(ApplicationDbContext context, ICurrentUser currentUser, ILogService logService)
    {
        _context = context;
        _currentUser = currentUser;
        _logService = logService;
    }

    [HttpPost("approve")]
    public async Task<IActionResult> Approve([FromBody] ApproveDebt args)
    {
        if (!User.IsInRole(RoleName.Accountant) && !User.IsInRole(RoleName.ChiefAccountant)) return BadRequest("Bạn không có quyền thực hiện chức năng này");

        try
        {
            var debt = await _context.UserTopups.FindAsync(args.Id);
            if (debt is null) return BadRequest("Không tìm thấy khoản thanh toán!");
            if (User.IsInRole(RoleName.Dos))
            {
                debt.Status = TopupStatus.DirectorApproved;
                debt.DirectorApprovedDate = DateTime.Now;
                debt.DirectorId = _currentUser.GetId();
                await _logService.AddAsync($"Giám đốc phê duyệt khoản thanh toán cho {debt.CardHolderId} ({debt.SaleId})");
            }
            else
            {
                debt.Status = TopupStatus.AccountantApproved;
                debt.AccountantApprovedDate = DateTime.Now;
                debt.AccountantId = _currentUser.GetId();
                await _logService.AddAsync($"Kết toán phê duyệt khoản thanh toán cho {debt.CardHolderId} ({debt.SaleId})");
            }
            debt.ModifiedDate = DateTime.Now;
            debt.ModifiedBy = _currentUser.GetId();
            debt.Note = args.Note;
            _context.UserTopups.Update(debt);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            await _logService.AddAsync($"Lỗi phê duyệt khoản thanh toán cho {args.Id}: {ex}");
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("reject")]
    public async Task<IActionResult> Reject([FromBody] ApproveDebt args)
    {
        if (!User.IsInRole(RoleName.Accountant) && !User.IsInRole(RoleName.ChiefAccountant)) return BadRequest("Bạn không có quyền thực hiện chức năng này");
        try
        {
            var debt = await _context.UserTopups.FindAsync(args.Id);
            if (debt is null) return BadRequest("Không tìm thấy khoản thanh toán!");
            if (User.IsInRole(RoleName.Dos))
            {
                debt.DosId = _currentUser.GetId();
                debt.DirectorApprovedDate = DateTime.Now;
                await _logService.AddAsync($"Giám đốc từ chối khoản thanh toán cho {debt.CardHolderId} ({debt.SaleId})");
            }
            else
            {
                debt.AccountantApprovedDate = DateTime.Now;
                debt.AccountantId = _currentUser.GetId();
                await _logService.AddAsync($"Kế toán từ chối khoản thanh toán cho {debt.CardHolderId} ({debt.SaleId})");
            }
            debt.Status = TopupStatus.Rejected;
            debt.ModifiedDate = DateTime.Now;
            debt.ModifiedBy = _currentUser.GetId();
            debt.Note = args.Note;
            _context.UserTopups.Update(debt);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            await _logService.AddAsync($"Lỗi từ chối khoản thanh toán cho {args.Id}: {ex}");
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("status-options")]
    public IActionResult StatusOptions()
    {
        var options = Enum.GetValues(typeof(TopupStatus))
            .Cast<TopupStatus>()
            .Select(x => new { value = x, Name = EnumHelper.GetDisplayName(x) })
            .ToList();
        return Ok(options);
    }
}
