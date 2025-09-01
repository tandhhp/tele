using Microsoft.AspNetCore.Mvc;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Core.Services.Contacts.Models;
using Microsoft.AspNetCore.Identity;
using Waffle.Core.Interfaces.IService;
using Waffle.Extensions;
using Waffle.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Waffle.Models;
using Waffle.Core.Constants;
using Waffle.Entities.Contacts;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Waffle.Core.Helpers;
using Waffle.Models.Args;
using Waffle.Core.Foundations;

namespace Waffle.Controllers;

public class ContactController(UserManager<ApplicationUser> _userManager,
    INotificationService _notificationService,
    ILogService _appLogService, ApplicationDbContext _context, IUserService _userService, IContactService _contactService) : BaseController
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] ContactFilterOptions filterOptions) => Ok(await _userService.ListAsync(filterOptions));

    [HttpGet("statistics")]
    public async Task<IActionResult> StatisticsAsync()
    {
        var totalContacts = await _context.Contacts.CountAsync();
        var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var previousMonth = currentMonth.AddMonths(-1);
        var totalCurrentMonth = await _context.Contacts.CountAsync(x => x.CreatedDate >= currentMonth);
        var totalPreviousMonth = await _context.Contacts.CountAsync(x => x.CreatedDate >= previousMonth && x.CreatedDate < currentMonth);
        var currentYear = new DateTime(DateTime.Now.Year, 1, 1);
        var totalCurrentYear = await _context.Contacts.CountAsync(x => x.CreatedDate >= currentYear);
        return Ok(TResult<object>.Ok(new
        {
            totalContacts,
            totalCurrentMonth,
            totalPreviousMonth,
            totalCurrentYear
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id) => Ok(TResult<object>.Ok(await _context.Contacts.FindAsync(id)));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact is null) return BadRequest("Không tìm thấy liên hệ!");
        _context.Contacts.Remove(contact);
        var activities = await _context.ContactActivities.Where(x => x.ContactId == id).ToListAsync();
        if (activities.Any())
        {
            _context.ContactActivities.RemoveRange(activities);
        }

        var admin = await _context.Users.FindAsync(User.GetId());
        if (admin is null) return BadRequest("Bạn không có quyền thực hiện điều này!");

        await _appLogService.AddAsync($"{admin.Name} - {admin.UserName} đã xóa liên hệ: {contact.Name} - {contact.PhoneNumber}", id);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateContactArgs args) => Ok(await _contactService.CreateContactAsync(args));

    [HttpGet("activity/list/{id}")]
    public async Task<IActionResult> ListActivityAsync([FromRoute] Guid id)
    {
        var query = _context.ContactActivities.Where(x => x.ContactId == id).OrderByDescending(x => x.CalledDate);
        return Ok(new
        {
            data = await query.ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("activity/add")]
    public async Task<IActionResult> AddActivityAsync([FromBody] ContactActivity args)
    {
        await _context.ContactActivities.AddAsync(args);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("activity/update")]
    public async Task<IActionResult> UpdateActivityAsync([FromBody] ContactActivity args)
    {
        var activity = await _context.ContactActivities.FindAsync(args.Id);
        if (activity is null)
        {
            return BadRequest("Activity not found!");
        }
        activity.Note = args.Note;
        activity.CalledDate = args.CalledDate;
        _context.ContactActivities.Update(activity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("activity/delete/{id}")]
    public async Task<IActionResult> DeleteActivtyAcync([FromRoute] Guid id)
    {
        var activity = await _context.ContactActivities.FindAsync(id);
        if (activity is null)
        {
            return BadRequest("Activity not found!");
        }
        _context.ContactActivities.Remove(activity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("lead/add")]
    public async Task<IActionResult> AddLeadAsync([FromBody] Lead args)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(args.PhoneNumber)) return BadRequest("Vui lòng nhập số điện thoại");
            var phoneNumber = args.PhoneNumber.Trim();
            if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(phoneNumber)) return BadRequest("Số điện thoại không hợp lệ");
            var lead = await _context.Leads.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            var status = "đã tồn tại";
            if (lead != null)
            {
                if (lead.Status == LeadStatus.LeadAccept || lead.Status == LeadStatus.AccountantApproved || lead.Status == LeadStatus.DosApproved || lead.Status == LeadStatus.Done)
                {
                    status = "đã có deal";
                }
                if (lead.Status == LeadStatus.LeadReject)
                {
                    status = "đã từ chối";
                }
                return BadRequest($"Khách hàng {lead.Name} với SDT {args.PhoneNumber} {status}, ngày tham gia {lead.EventDate?.ToString("dd-MM-yyyy")} - {lead.EventTime}!");
            }

            if (!string.IsNullOrWhiteSpace(args.IdentityNumber))
            {
                lead = await _context.Leads.FirstOrDefaultAsync(x => x.IdentityNumber == args.IdentityNumber);
                if (lead != null)
                {
                    if (lead.Status == LeadStatus.LeadAccept || lead.Status == LeadStatus.AccountantApproved || lead.Status == LeadStatus.DosApproved || lead.Status == LeadStatus.Done)
                    {
                        status = "đã có deal";
                    }
                    if (lead.Status == LeadStatus.LeadReject)
                    {
                        status = "đã từ chối";
                    }
                    return BadRequest($"Khách hàng {lead.Name} với CCCD {lead.IdentityNumber} {status}, ngày tham gia {lead.EventDate?.ToString("dd-MM-yyyy")} - {lead.EventTime}!");
                }
            }
            var cardHolder = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (cardHolder != null) return BadRequest($"{cardHolder.Name} - {cardHolder.PhoneNumber} đã là chủ thẻ!");
            var note = string.Empty;
            args.Status = LeadStatus.Pending;
            args.PhoneNumber = phoneNumber;
            args.CreatedDate = DateTime.Now;
            if (User.IsInRole(RoleName.Sales))
            {
                args.SalesId = User.GetId();
                var sales = await _context.Users.FindAsync(args.SalesId);
                if (sales is null) return BadRequest("Không tìm thấy trợ lý cá nhân!");
                args.Branch = sales.Branch;
                args.TelesaleId = sales.Id;
                args.TmId = sales.SmId;
                await _notificationService.CreateAsync($"Key-In {args.Name} - {args.PhoneNumber} cần được phê duyệt!", $"Khách hàng {args.Name} - {args.PhoneNumber} đã được tạo mới bởi {sales.Name} cần được bạn phê duyệt!", args.TmId);
            }
            if (User.IsInRole(RoleName.Telesale))
            {
                args.TelesaleId = User.GetId();
                var telesale = await _context.Users.FindAsync(args.TelesaleId);
                if (telesale is null || telesale.TmId is null) return BadRequest("Không tìm thấy Tele Manager!");
                args.TmId = telesale.TmId;
                note = "Telesale tạo mới";
            }
            if (args.SalesId != null)
            {
                var sale = await _userManager.FindByIdAsync(args.SalesId.GetValueOrDefault().ToString());
                if (sale is null) return BadRequest("Sale not found!");
                args.Branch = sale.Branch;
            }
            if (User.IsInRole(RoleName.TelesaleManager))
            {
                args.Status = LeadStatus.Approved;
                note = $"Telesale Manager tạo mới Key-In";
            }
            if (User.IsInRole(RoleName.Event))
            {
                args.Status = LeadStatus.Approved;
                note = $"Event tạo mới Key-In";
            }
            args.CreatedBy = User.GetId();
            await _context.Leads.AddAsync(args);
            if (args.SubLeads != null)
            {
                foreach (var item in args.SubLeads)
                {
                    if (string.IsNullOrEmpty(item.Name)) return BadRequest("Vui lòng nhập tên khách hàng tiềm năng đi cùng");
                    item.LeadId = args.Id;
                    await _context.SubLeads.AddAsync(item);
                }
            }

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = args.Id,
                Status = args.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = note
            });

            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("lead/update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Lead args)
    {
        try
        {
            var lead = await _context.Leads.FindAsync(args.Id);
            if (lead is null) return BadRequest();

            var user = await _context.Users.FindAsync(User.GetId());
            if (user is null) return Unauthorized();

            var cardHolder = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == args.PhoneNumber);
            if (cardHolder != null)
            {
                return BadRequest($"{cardHolder.Name} - {cardHolder.PhoneNumber} đã là chủ thẻ!");
            }
            if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(args.PhoneNumber)) return BadRequest("Số điện thoại không hợp lệ");

            lead.PhoneNumber = args.PhoneNumber;
            lead.Email = args.Email;
            lead.Address = args.Address;
            lead.DateOfBirth = args.DateOfBirth;
            lead.EventTime = args.EventTime;
            lead.EventDate = args.EventDate;
            lead.Gender = args.Gender;
            if (!string.IsNullOrWhiteSpace(lead.IdentityNumber))
            {
                if (lead.IdentityNumber != args.IdentityNumber)
                {
                    var exist = await _context.Leads.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.IdentityNumber) && x.IdentityNumber == args.IdentityNumber);
                    if (exist != null)
                    {
                        return BadRequest($"CCCD đã tồn tại Key-in: {exist.Name} - {exist.PhoneNumber}!");
                    }
                }
            }
            lead.IdentityNumber = args.IdentityNumber;
            lead.Note = args.Note;
            lead.Branch = args.Branch;
            lead.Name = args.Name;
            lead.TelesaleId = args.TelesaleId;
            if (lead.SalesId != null)
            {
                var sale = await _userManager.FindByIdAsync(lead.SalesId.GetValueOrDefault().ToString());
                if (sale is null) return BadRequest("Sale not found!");
                lead.Branch = sale.Branch;
                lead.DosId = sale.DosId;
            }
            if (args.TelesaleId != null)
            {
                var tele = await _context.Users.FindAsync(args.TelesaleId);
                if (tele is null) return BadRequest("Tele not found!");
                lead.TmId = tele.TmId;
                lead.DotId = tele.DotId;
            }
            if (User.IsInRole(RoleName.Sales))
            {
                lead.SalesId = user.Id;
            }
            _context.Leads.Update(lead);

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = args.Id,
                Status = args.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = "Cập nhật thông tin"
            });

            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("sublead/remove/{id}")]
    public async Task<IActionResult> RemoveSubLeadAsync([FromRoute] Guid id)
    {
        var subLead = await _context.Leads.FindAsync(id);
        if (subLead is null) return BadRequest("Không tìm thấy khách hàng tiềm năng đi cùng!");
        _context.Leads.Remove(subLead);

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = subLead.Id,
            Status = LeadStatus.Done,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = "Xóa người đi cùng"
        });

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("lead/list")]
    public async Task<IActionResult> ListLeadAsync([FromQuery] LeadFilterOptions filterOptions)
    {
        var user = await _context.Users.FindAsync(User.GetId());
        if (user is null) return BadRequest();
        var query = from a in _context.Leads
                    join b in _context.Users on a.SalesId equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in _context.Users on a.TelesaleId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.CreatedDate,
                        a.Name,
                        a.Email,
                        a.PhoneNumber,
                        a.DateOfBirth,
                        a.EventTime,
                        a.Address,
                        a.Status,
                        SalesName = b.Name,
                        b.SmId,
                        b.DosId,
                        a.SalesId,
                        a.EventDate,
                        SaleName = b.Name,
                        a.Gender,
                        a.Branch,
                        TeleName = c.Name,
                        a.TelesaleId,
                        a.TmId,
                        a.Note,
                        inviteCount = _context.LeadHistories.Count(x => x.LeadId == a.Id) + 1
                    };
        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == user.Id);
        }
        if (User.IsInRole(RoleName.Sales))
        {
            query = query.Where(x => x.SalesId == user.Id);
        }
        if (User.IsInRole(RoleName.Telesale))
        {
            query = query.Where(x => x.TelesaleId == user.Id);
        }
        if (User.IsInRole(RoleName.TelesaleManager))
        {
            var teleIds = await (from a in _context.Users
                                 join b in _context.UserRoles on a.Id equals b.UserId
                                 join c in _context.Roles on b.RoleId equals c.Id
                                 where c.Name == RoleName.Telesale && a.TmId == user.Id
                                 select a.Id).ToListAsync();
            query = query.Where(x => x.TelesaleId != null && teleIds.Contains(x.TelesaleId.Value));
        }
        if (User.IsInRole(RoleName.Dot))
        {
            var tmIds = await (from a in _context.Users
                               join b in _context.UserRoles on a.Id equals b.UserId
                               join c in _context.Roles on b.RoleId equals c.Id
                               where c.Name == RoleName.TelesaleManager && a.DotId == user.Id
                               select a.Id).ToListAsync();

            var teleIds = await (from a in _context.Users
                                 join b in _context.UserRoles on a.Id equals b.UserId
                                 join c in _context.Roles on b.RoleId equals c.Id
                                 where c.Name == RoleName.Telesale && a.TmId != null && tmIds.Contains(a.TmId.Value)
                                 select a.Id).ToListAsync();

            var telesales = await _userManager.GetUsersInRoleAsync(RoleName.Telesale);

            query = query.Where(x => x.TelesaleId != null && teleIds.Contains(x.TelesaleId.Value));
        }
        if (!string.IsNullOrEmpty(filterOptions.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber == filterOptions.PhoneNumber);
        }
        if (!string.IsNullOrEmpty(filterOptions.Email))
        {
            query = query.Where(x => x.Email == filterOptions.Email);
        }
        if (!string.IsNullOrEmpty(filterOptions.Name))
        {
            query = query.Where(x => x.Name == filterOptions.Name);
        }
        if (filterOptions.EventDate != null)
        {
            query = query.Where(x => x.EventDate != null && x.EventDate.Value.Date == filterOptions.EventDate.Value.Date);
        }
        if (filterOptions.FromDate != null && filterOptions.ToDate != null)
        {
            query = query.Where(x => x.EventDate != null && x.EventDate.Value.Date >= filterOptions.FromDate.Value.Date && x.EventDate <= filterOptions.ToDate.Value.Date);
        }
        if (!User.IsInRole(RoleName.Telesale) && !User.IsInRole(RoleName.TelesaleManager) && !User.IsInRole(RoleName.Dot) && !User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.CxTP))
        {
            query = query.Where(x => x.Branch == user.Branch);
        }
        if (filterOptions.Branch != null)
        {
            query = query.Where(x => x.Branch == filterOptions.Branch);
        }
        if (!string.IsNullOrEmpty(filterOptions.EventTime))
        {
            query = query.Where(x => x.EventTime == filterOptions.EventTime);
        }
        if (filterOptions.SmId != null)
        {
            query = query.Where(x => x.SmId == filterOptions.SmId);
        }

        return Ok(new
        {
            data = await query.OrderByDescending(x => x.EventDate).ThenBy(x => x.EventTime).Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpGet("subleads/{id}")]
    public async Task<IActionResult> ListSubLeadAsync([FromRoute] Guid id)
    {
        var query = _context.SubLeads.Where(x => x.LeadId == id);
        return Ok(new
        {
            data = await query.ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("lead/delete/{id}")]
    public async Task<IActionResult> DeleteLeadAsync([FromRoute] Guid id)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead == null) return BadRequest();
        if (!User.IsInRole(RoleName.Admin))
        {
            return BadRequest("Bạn không có quyền!");
        }

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = lead.Id,
            Status = lead.Status,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = "Xóa Key-in"
        });

        _context.Leads.Remove(lead);

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("lead/status")]
    public async Task<IActionResult> ChangeStatusLeadAsync([FromBody] Lead args)
    {
        var lead = await _context.Leads.FindAsync(args.Id);
        if (lead is null) return BadRequest();
        lead.Status = args.Status;
        if (args.Status == LeadStatus.AccountantApproved)
        {
            if (!User.IsInRole(RoleName.Accountant) && !User.IsInRole(RoleName.ChiefAccountant)) return BadRequest("Chỉ kế toán được phê duyệt!");
            lead.AccountantId = User.GetId();
            lead.AccountantApprovedDate = DateTime.Now;

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = lead.Id,
                Status = lead.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = "Kế toán xác nhận"
            });
        }
        if (args.Status == LeadStatus.DosApproved)
        {
            if (!User.IsInRole(RoleName.Dos)) return BadRequest("Chỉ giám đốc được phê duyệt!");
            lead.DosId = User.GetId();
            lead.DosApprovedDate = DateTime.Now;

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = lead.Id,
                Status = lead.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = "Giám đốc phê duyệt"
            });
            var accountant = from a in _context.Users
                             join b in _context.UserRoles on a.Id equals b.UserId
                             join c in _context.Roles on b.RoleId equals c.Id
                             where c.Name == RoleName.Accountant && a.Branch == lead.Branch
                             select a.Id;
            await _notificationService.CreateAsync($"Key-in {lead.Name} - {lead.PhoneNumber} đã được phê duyệt!", $"Khách hàng {lead.Name} - {lead.PhoneNumber} đã được phê duyệt bởi {User.GetUserName()}, vui lòng kiểm tra và xác nhận!", await accountant.FirstOrDefaultAsync());
        }
        if (args.Status == LeadStatus.Checkin)
        {
            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = lead.Id,
                Status = lead.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = "Khách hàng check-in"
            });
        }
        _context.Leads.Update(lead);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("lead-options")]
    public async Task<IActionResult> LeadOptionsAsync([FromQuery] string time) => Ok(await _context.Leads
        .Where(x => x.Status == LeadStatus.Approved)
        .Where(x => x.EventTime == time).Select(x => new
        {
            label = x.Name,
            value = x.Id
        }).ToListAsync());

    [HttpGet("lead/{id}")]
    public async Task<IActionResult> GetLeadDetailAsync([FromRoute] Guid id)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead is null) return BadRequest("Không tìm thấy khách hàng tiềm năng");
        return Ok(new
        {
            data = new
            {
                lead.Id,
                lead.EventDate,
                lead.EventTime,
                lead.Name,
                lead.Status,
                lead.Address,
                lead.Email,
                lead.Branch,
                lead.CreatedDate,
                lead.DateOfBirth,
                lead.Gender,
                lead.IdentityNumber,
                lead.PhoneNumber,
                lead.SalesId,
                SubLeads = await _context.SubLeads.Where(x => x.LeadId == lead.Id).ToListAsync()
            }
        });
    }

    [HttpGet("users-in-event")]
    public async Task<IActionResult> UsersInEventAsync([FromQuery] LeadFilterOptions args)
    {
        try
        {
            var user = await _context.Users.FindAsync(User.GetId());
            if (user is null) return Unauthorized();
            var query = from a in _context.Leads
                        join e in _context.Users on a.AccountantId equals e.Id into ae
                        from e in ae.DefaultIfEmpty()
                        join f in _context.Users on a.TelesaleId equals f.Id into af
                        from f in af.DefaultIfEmpty()
                        join g in _context.Users on a.TelesaleId equals g.Id into ag
                        from g in ag.DefaultIfEmpty()
                        where string.IsNullOrEmpty(args.EventTime) || a.EventTime == args.EventTime
                        select new
                        {
                            a.Id,
                            a.EventDate,
                            a.Gender,
                            a.CreatedDate,
                            a.Name,
                            a.EventTime,
                            a.Address,
                            a.Branch,
                            a.Status,
                            a.PhoneNumber,
                            a.DateOfBirth,
                            a.Email,
                            a.IdentityNumber,
                            a.SalesId,
                            SubLeads = _context.SubLeads.Count(x => x.LeadId == a.Id),
                            SalesName = _context.Users.First(x => x.Id == a.SalesId).Name,
                            AccountantName = e.Name,
                            a.AccountantId,
                            a.TelesaleId,
                            a.TmId,
                            a.Note,
                            TeleName = g.Name ?? f.Name,
                            SubLeadName = string.Join(",", _context.SubLeads.Where(x => x.LeadId == a.Id).Select(x => x.Name)),
                            inviteCount = _context.LeadHistories.Count(x => x.LeadId == a.Id) + 1
                        };
            if (args.EventDate != null)
            {
                query = query.Where(x => x.EventDate != null && x.EventDate.Value.Date == args.EventDate.Value.Date);
            }
            if (args.Status != null)
            {
                query = query.Where(x => x.Status == args.Status);
            }
            if (!string.IsNullOrWhiteSpace(args.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(args.Name.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(args.PhoneNumber))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(args.PhoneNumber));
            }
            if (!string.IsNullOrEmpty(args.Email))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(args.Email.ToLower()));
            }
            if (User.IsInRole(RoleName.Sales))
            {
                query = query.Where(x => x.SalesId == user.Id);
            }
            if (User.IsInRole(RoleName.Telesale))
            {
                query = query.Where(x => x.TelesaleId == user.Id);
            }
            if (User.IsInRole(RoleName.TelesaleManager))
            {
                if (User.IsInRole(RoleName.TelesaleManager))
                {
                    var teleIds = await (from a in _context.Users
                                         join b in _context.UserRoles on a.Id equals b.UserId
                                         join c in _context.Roles on b.RoleId equals c.Id
                                         where c.Name == RoleName.Telesale && a.TmId == user.Id
                                         select a.Id).ToListAsync();
                    query = query.Where(x => x.TelesaleId != null && teleIds.Contains(x.TelesaleId.Value));
                }
            }
            if (User.IsInRole(RoleName.SalesManager))
            {
                var salesIds = await _userManager.Users.Where(x => x.SmId == user.Id).Select(x => x.Id).ToListAsync();
                if (salesIds.Any())
                {
                    query = query.Where(x => x.SalesId != null && salesIds.Contains(x.SalesId.Value));
                }
            }
            if (args.Branch != null)
            {
                query = query.Where(x => x.Branch == args.Branch);
            }

            // Chỉ cho phép nhìn thấy đội telesales
            if (User.IsInRole(RoleName.Dot))
            {
                var teles = await _userManager.GetUsersInRoleAsync(RoleName.Telesale);
                var teleIds = teles.Where(x => x.DotId == user.Id).Select(x => x.Id).ToList();
                query = query.Where(x => x.TelesaleId != null && teleIds.Contains(x.TelesaleId.Value));
            }
            if (args.InQueue == true)
            {
                query = query.Where(x => x.Status == LeadStatus.LeadAccept || x.Status == LeadStatus.LeadReject || x.Status == LeadStatus.Done || x.Status == LeadStatus.AccountantApproved || x.Status == LeadStatus.DosApproved);
            }
            // admin và trưởng phòng cx được thấy cả 2 miền
            if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.CxTP) && !User.IsInRole(RoleName.ChiefAccountant) && !User.IsInRole(RoleName.Telesale) && !User.IsInRole(RoleName.TelesaleManager) && !User.IsInRole(RoleName.Dot))
            {
                query = query.Where(x => x.Branch == user.Branch);
            }
            var data = await query.OrderByDescending(x => x.EventDate).Skip((args.Current - 1) * args.PageSize).AsNoTracking().Take(args.PageSize).ToListAsync();

            return Ok(new
            {
                data,
                total = await query.CountAsync()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
    
    [HttpGet("card-holder-queue-list")]
    public async Task<IActionResult> GetCardHolderQueueListAsync([FromQuery] UserFilterOptions filterOptions)
    {
        var query = from a in _context.CardHolderQueues
                    join b in _context.Users on a.CardHolderId equals b.Id
                    join c in _context.Users on a.ChangedBy equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    join d in _context.Users on a.RequestBy equals d.Id
                    join e in _context.Cards on b.CardId equals e.Id
                    select new
                    {
                        a.Id,
                        a.RequestDate,
                        a.ChangedDate,
                        a.Status,
                        CardHolderName = b.Name,
                        b.ContractCode,
                        e.Tier,
                        b.Loyalty,
                        ChangedBy = c.Name,
                        RequestBy = d.Name,
                        b.PhoneNumber,
                        b.Email,
                        b.Amount,
                        a.CardHolderId,
                        b.DosId,
                        b.SmId,
                        b.SellerId,
                        b.Branch
                    };
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user == null) return Unauthorized();
        if (!string.IsNullOrEmpty(filterOptions.Name))
        {
            query = query.Where(x => x.CardHolderName.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        if (!string.IsNullOrEmpty(filterOptions.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber.ToLower()));
        }
        if (!string.IsNullOrEmpty(filterOptions.Email))
        {
            query = query.Where(x => x.Email.ToLower().Contains(filterOptions.Email.ToLower()));
        }
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == user.Id);
        }
        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Sales))
        {
            query = query.Where(x => x.SellerId == user.Id);
        }
        return Ok(new
        {
            data = await query.OrderByDescending(x => x.RequestDate).Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("card-holder-queue-status")]
    public async Task<IActionResult> CardHolderQueueStatusAsync([FromBody] CardHolderQueue args)
    {
        var chq = await _context.CardHolderQueues.FindAsync(args.Id);
        if (chq is null) return BadRequest("Không tìm thấy yêu cầu!");
        chq.Status = args.Status;
        chq.ChangedDate = DateTime.Now;
        chq.ChangedBy = User.GetId();
        _context.CardHolderQueues.Update(chq);
        var cardHolder = await _context.Users.FindAsync(chq.CardHolderId);
        if (cardHolder is null) return BadRequest("Không tìm thấy chủ thẻ");

        if (!User.IsInRole(RoleName.Dos)) return BadRequest("DOS mới có quyền phê duyệt");

        if (args.Status == CardHolderQueueStatus.Approved)
        {
            if (cardHolder.Amount < 1) return BadRequest("Vui lòng nhập công nợ cho chủ thẻ!");

            cardHolder.HasChange = false;
            await _userService.SendEmailToCardHolderAsync(new CreateUserModel
            {
                UserName = cardHolder.UserName,
                Name = cardHolder.Name,
                Password = $"nuras@{cardHolder.UserName}"
            });
        }
        if (args.Status == CardHolderQueueStatus.Rejected)
        {
            //_context.Users.Remove(cardHolder);
            var user = await _userManager.FindByIdAsync(User.GetClaimId());
            if (user is null) return Unauthorized();
            await _appLogService.AddAsync($"{user.Name} đã từ chối yêu cầu chuyển đổi khách hàng tiềm năng {cardHolder.Name} - {cardHolder.ContractCode}");
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("sublead/add")]
    public async Task<IActionResult> AddSubLeadAsync([FromBody] SubLead args)
    {
        var phoneNumber = args.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(args.PhoneNumber))
        {
            phoneNumber = args.PhoneNumber.Trim();
            if (phoneNumber.Length != 10)
            {
                return BadRequest("Số điện thoại không hợp lệ");
            }
            if (await _context.SubLeads.AnyAsync(x => x.PhoneNumber == phoneNumber))
            {
                return BadRequest("Số điện thoại đã tồn tại");
            }
        }
        args.PhoneNumber = phoneNumber;
        await _context.SubLeads.AddAsync(args);

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = args.LeadId,
            Status = LeadStatus.Done,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = "Thêm người đi cùng"
        });

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("sublead/update")]
    public async Task<IActionResult> UpdateSubLeadAsync([FromBody] SubLead args)
    {
        var sublead = await _context.SubLeads.FindAsync(args.Id);
        if (sublead is null) return BadRequest("Không tìm thấy người đi cùng");
        var phoneNumber = args.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(args.PhoneNumber))
        {
            if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(args.PhoneNumber)) return BadRequest("Số điện thoại không hợp lệ");
        }
        sublead.PhoneNumber = phoneNumber;
        sublead.IdentityNumber = args.IdentityNumber;
        sublead.Address = args.Address;
        sublead.Gender = args.Gender;
        sublead.Name = args.Name;
        _context.SubLeads.Update(sublead);

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = sublead.LeadId,
            Status = LeadStatus.Done,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = "Cập nhật người đi cùng"
        });

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("sublead/delete/{id}")]
    public async Task<IActionResult> DeleteSubLeadAsync([FromRoute] Guid id)
    {
        var sublead = await _context.SubLeads.FindAsync(id);
        if (sublead is null) return BadRequest("Sub Lead not found!");

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = sublead.LeadId,
            Status = LeadStatus.Done,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = "Xóa người đi cùng"
        });

        _context.SubLeads.Remove(sublead);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("my-keyin-list")]
    public async Task<IActionResult> GetMyKeyInAsync([FromQuery] UserFilterOptions filterOptions)
    {
        var userId = User.GetId();
        var query = from a in _context.Leads
                    join b in _context.Users on a.SalesId equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in _context.Users on a.TelesaleId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.EventDate,
                        a.EventTime,
                        a.CreatedDate,
                        a.Gender,
                        a.Name,
                        a.Address,
                        a.Branch,
                        a.DateOfBirth,
                        a.Email,
                        a.PhoneNumber,
                        a.TelesaleId,
                        a.Status,
                        a.SalesId,
                        SaleName = b.Name,
                        TeleName = c.Name,
                        inviteCount = _context.LeadHistories.Count(x => x.LeadId == a.Id) + 1
                    };
        if (filterOptions.Branch != null)
        {
            query = query.Where(x => x.Branch == filterOptions.Branch);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(filterOptions.PhoneNumber));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        if (!User.IsInRole(RoleName.Event))
        {
            query = query.Where(x => x.TelesaleId == userId || x.SalesId == userId);
        }

        return Ok(new
        {
            data = await query.OrderByDescending(x => x.EventDate).Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("move-event-date")]
    public async Task<IActionResult> MoveEventDateAsync([FromBody] Lead args)
    {
        try
        {
            var lead = await _context.Leads.FindAsync(args.Id);
            if (lead is null) return BadRequest("Lead not found!");
            if (lead.Status == LeadStatus.LeadAccept)
            {
                return BadRequest("Khách đã chốt deal không thể di chuyển ngày!");
            }

            if (lead.Status != LeadStatus.Approved && lead.Status != LeadStatus.Pending)
            {
                lead.Status = LeadStatus.ReInvite;
                await _context.LeadHistories.AddAsync(new LeadHistory
                {
                    LeadId = lead.Id,
                    EventDate = lead.EventDate,
                    EventTime = lead.EventTime,
                    Note = lead.Note,
                    SalesId = lead.SalesId,
                    TelesaleId = lead.TelesaleId
                });
            }

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = lead.Id,
                Status = lead.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = $"Chuyển ngày sự kiện từ {lead.EventDate?.ToString("dd-MM-yyyy")} {lead.EventTime} sang {lead.EventDate?.ToString("dd-MM-yyyy")} {lead.EventTime}"
            });


            if (!User.IsInRole(RoleName.Event))
            {
                var eventIds = await (from a in _context.Users
                                      join b in _context.UserRoles on a.Id equals b.UserId
                                      join c in _context.Roles on b.RoleId equals c.Id
                                      where c.Name == RoleName.Event && a.Branch == lead.Branch
                                      select a.Id).ToListAsync();

                await _notificationService.CreateAsync(
                    $"Key-In {lead.Name} đổi ngày sự kiện",
                    $"{User.GetUserName()} đã đổi ngày key-In {lead.Name} - {lead.PhoneNumber} tham gia sự kiện từ ngày {lead.EventDate} {lead.EventTime} sang {args.EventDate} {lead.EventTime}.",
                    eventIds);
            }

            lead.EventDate = args.EventDate;
            lead.EventTime = args.EventTime;
            lead.Note = args.Note;

            _context.Leads.Update(lead);

            await _context.SaveChangesAsync();

            return Ok(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("list-lead-history/{id}")]
    public async Task<IActionResult> ListLeadHistoryAsync([FromQuery] LeadFilterOptions filterOptions, [FromRoute] Guid id)
    {
        var query = from a in _context.LeadHistories
                    join b in _context.Leads on a.LeadId equals b.Id
                    where a.LeadId == id
                    select new
                    {
                        a.Id,
                        a.LeadId,
                        a.EventDate,
                        a.EventTime,
                        a.Note,
                        a.TelesaleId,
                        a.SalesId,
                        TeleName = _context.Users.First(x => x.Id == a.TelesaleId).Name,
                        SaleName = _context.Users.First(x => x.Id == a.SalesId).Name,
                        b.PhoneNumber,
                        b.Name,
                        a.TableStatus
                    };
        return Ok(new
        {
            data = await query.OrderByDescending(x => x.EventDate).Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("export-lead")]
    public async Task<IActionResult> ExportLeadAsync([FromBody] ExportDateFilterOptions filterOptions)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(User.GetId().ToString());
            if (user is null) return Unauthorized();
            var query = from a in _context.Leads
                        join b in _context.Users on a.TelesaleId equals b.Id into ab
                        from b in ab.DefaultIfEmpty()
                        join d in _context.Users on a.SalesId equals d.Id into ad
                        from d in ad.DefaultIfEmpty()
                        where a.Branch == user.Branch && a.Status != LeadStatus.Pending && a.Status != LeadStatus.Approved
                        select new
                        {
                            a.Id,
                            a.Name,
                            a.PhoneNumber,
                            a.EventDate,
                            a.EventTime,
                            TeleName = b.Name,
                            SalesName = d.Name,
                            a.Note
                        };
            if (filterOptions.FromDate != null)
            {
                query = query.Where(x => x.EventDate >= filterOptions.FromDate);
            }
            if (filterOptions.ToDate != null)
            {
                query = query.Where(x => x.EventDate <= filterOptions.ToDate);
            }

            var data = await query.ToListAsync();

            using var pgk = new ExcelPackage();
            var ws = pgk.Workbook.Worksheets.Add("Sheet1");

            ws.Cells[1, 1].Value = "STT";
            ws.Cells[1, 2].Value = "Họ và tên";
            ws.Cells[1, 3].Value = "SDT";
            ws.Cells[1, 4].Value = "Trạng thái";
            ws.Cells[1, 5].Value = "Ngày tham gia";
            ws.Cells[1, 6].Value = "Giờ tham gia";
            ws.Cells[1, 7].Value = "Người gọi";
            ws.Cells[1, 8].Value = "Nguồn";
            ws.Cells[1, 9].Value = "Số hợp đồng";
            ws.Cells[1, 10].Value = "Giờ check-in";
            ws.Cells[1, 11].Value = "Giờ check-out";
            ws.Cells[1, 12].Value = "Bước";
            ws.Cells[1, 13].Value = "Người tiếp";
            ws.Cells[1, 14].Value = "Ghi chú";

            var row = 2;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = row - 1;
                ws.Cells[row, 2].Value = item.Name;
                ws.Cells[row, 3].Value = item.PhoneNumber;
                ws.Cells[row, 5].Value = item.EventDate?.ToString("dd-MM-yyyy");
                ws.Cells[row, 6].Value = item.EventTime;
                ws.Cells[row, 7].Value = item.TeleName;
                ws.Cells[row, 13].Value = item.SalesName;
                ws.Cells[row, 14].Value = item.Note;

                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            var dataRange = ws.Cells[1, 1, row, 15];
            // Apply borders to the entire data range
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            for (int i = 1; i < 15; i++)
            {
                ws.Column(i).AutoFit();
            }

            await pgk.SaveAsync();

            var fileName = $"data-keyin-nuras";

            return File(await pgk.GetAsByteArrayAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("change-lead-phone-number")]
    public async Task<IActionResult> ChangeLeadPhoneNumberAsync([FromBody] Lead args)
    {
        var lead = await _context.Leads.FindAsync(args.Id);
        if (lead is null) return BadRequest("Data not found!");
        if (string.IsNullOrWhiteSpace(args.PhoneNumber))
        {
            return BadRequest("Vui lòng nhập số điện thoại!");
        }
        if (args.PhoneNumber.Length != 10) return BadRequest("Số điện thoại không hợp lệ");
        if (await _context.Leads.AnyAsync(x => x.PhoneNumber == args.PhoneNumber))
        {
            return BadRequest("Số điện thoại đã tồn tại!");
        }

        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = lead.Id,
            Status = lead.Status,
            CreatedDate = DateTime.Now,
            UserId = User.GetId(),
            Note = $"Đổi số điện thoại từ {lead.PhoneNumber} sang {args.PhoneNumber}"
        });

        lead.PhoneNumber = args.PhoneNumber;
        _context.Leads.Update(lead);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("change-tele")]
    public async Task<IActionResult> ChangeTeleAsync([FromBody] Lead args)
    {
        try
        {
            var lead = await _context.Leads.FindAsync(args.Id);
            if (lead is null) return BadRequest("Không tìm thấy Key-in");
            if (args.TelesaleId is null) return BadRequest("Vui lòng chọn người gọi!");
            var tele = await _userManager.FindByIdAsync(args.TelesaleId.GetValueOrDefault().ToString());
            if (tele is null) return BadRequest("Không tìm thấy người gọi!");
            lead.TelesaleId = tele.Id;
            _context.Leads.Update(lead);

            await _context.LeadProcesses.AddAsync(new LeadProcess
            {
                LeadId = lead.Id,
                Status = lead.Status,
                CreatedDate = DateTime.Now,
                UserId = User.GetId(),
                Note = $"Đổi người gọi Key-In sang {tele.Name} - {tele.UserName}"
            });

            await _appLogService.AddAsync($"Đổi người gọi Key-In: {lead.Name} sang {tele.Name} - {tele.UserName}");
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("lead-process")]
    public async Task<IActionResult> GetLeadProcessAsync([FromQuery] LeadProcessFilterOptions filterOptions)
    {
        var query = from a in _context.LeadProcesses
                    join b in _context.Leads on a.LeadId equals b.Id
                    join c in _context.Users on a.UserId equals c.Id
                    join d in _context.UserRoles on c.Id equals d.UserId
                    join e in _context.Roles on d.RoleId equals e.Id
                    select new
                    {
                        a.Id,
                        a.LeadId,
                        a.Status,
                        a.Note,
                        a.CreatedDate,
                        UserName = c.Name,
                        LeadName = b.Name,
                        b.PhoneNumber,
                        b.EventDate,
                        b.EventTime,
                        b.Gender,
                        b.Branch,
                        RoleName = e.DisplayName
                    };
        if (filterOptions.Branch != null)
        {
            query = query.Where(x => x.Branch == filterOptions.Branch);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Note))
        {
            query = query.Where(x => x.Note != null && x.Note.ToLower().Contains(filterOptions.Note.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.LeadName))
        {
            query = query.Where(x => x.LeadName != null && x.LeadName.ToLower().Contains(filterOptions.LeadName.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.UserName))
        {
            query = query.Where(x => x.UserName != null && x.UserName.ToLower().Contains(filterOptions.UserName.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpPost("change-status")]
    public async Task<IActionResult> ChangeStatusAsync([FromBody] ChangeStatusArgs args)
    {
        if (!User.IsInRole(RoleName.Admin)) return BadRequest("Không thể thay đổi trạng thái của Key-In");
        var lead = await _context.Leads.FindAsync(args.LeadId);
        if (lead is null) return BadRequest("Không tìm thấy Key-In");
        if (lead.Status == args.Status) return BadRequest("Trạng thái không thay đổi!");
        lead.Status = args.Status;
        _context.Leads.Update(lead);
        await _appLogService.AddAsync($"Thay đổi trạng thái Key-In: {lead.Name} từ {lead.Status} sang {args.Status}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("status-options")]
    public IActionResult GetStatusOptions()
    {
        var status = Enum.GetValues(typeof(LeadStatus)).Cast<LeadStatus>().Select(x => new
        {
            label = EnumHelper.GetDisplayName(x),
            value = x
        }).ToList();
        return Ok(status);
    }

    [HttpGet("blacklist")]
    public async Task<IActionResult> GetBlacklistAsync([FromQuery] BlacklistFilterOptions filterOptions) => Ok(await _contactService.GetBlacklistAsync(filterOptions));

    [HttpPost("block")]
    public async Task<IActionResult> BlockAsync([FromBody] BlockContactArgs args) => Ok(await _contactService.BlockAsync(args));

}
