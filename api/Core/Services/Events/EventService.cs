using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Constants;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IRepository.Events;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Interfaces.IService.Events;
using Waffle.Core.Services.Events.Models;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Services.Events;

public class EventService(ApplicationDbContext _context, IEventRepository _eventRepository, ILogService _logService, IRoomService _roomService, ICurrentUser _currentUser, UserManager<ApplicationUser> _userManager, IWebHostEnvironment webHostEnvironment) : IEventService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async Task<TResult> AddSaleRevenueAsync(AddSaleRevenue args)
    {
        var keyIn = await _context.Leads.FindAsync(args.LeadId);
        if (keyIn is null) return TResult.Failed("Không tìm thấy Key-In!");
        if (keyIn.SalesId is null) return TResult.Failed("Vui lòng chỉ định trợ lý cá nhân cho Key-In!");
        var sale = await _context.Users.FindAsync(keyIn.SalesId);
        if (sale is null) return TResult.Failed("Không tìm thấy trợ lý cá nhân!");
        if (sale.SmId is null) return TResult.Failed("Vui lòng chỉ định quản lý quan hệ khách hàng!");
        if (sale.DosId is null) return TResult.Failed("Vui lòng chỉ định giám đốc quan hệ khách hàng!");
        if (args.Amount <= 0) return TResult.Failed("Số tiền không hợp lệ!");

        var topup = new UserTopup
        {
            Id = Guid.NewGuid(),
            Amount = args.Amount,
            SaleId = keyIn.SalesId.GetValueOrDefault(),
            Status = TopupStatus.Pending,
            CreatedDate = DateTime.Now,
            SmId = sale.SmId,
            DosId = sale.DosId,
            Type = TopupType.Event,
            LeadId = args.LeadId,
            Note = args.Note,
            CreatedBy = _currentUser.GetId(),
            ContractCode = args.ContractCode
        };

        await _context.UserTopups.AddAsync(topup);

        if (args.Evidences != null)
        {
            foreach (var evidence in args.Evidences)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "evidence", args.ContractCode ?? "undefined");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                using var fileStream = new FileStream(Path.Combine(uploadPath, evidence.FileName), FileMode.Create);
                await evidence.CopyToAsync(fileStream);
                var topupEvidence = new TopupEvidence
                {
                    Id = Guid.NewGuid(),
                    TopupId = topup.Id,
                    FileUrl = "https://nuras.com.vn/evidence/" + args.ContractCode + "/" + evidence.FileName
                };
                await _context.TopupEvidences.AddAsync(topupEvidence);
            }
        }

        await _logService.AddAsync($"Thêm doanh thu sự kiện cho {sale.Name} ({sale.UserName})");
        return TResult.Success;
    }

    public async Task<TResult> BackToCheckinAsync(BackToCheckin args)
    {
        var keyIn = await _context.Leads.FindAsync(args.KeyInId);
        if (keyIn is null) return TResult.Failed("Không tìm thấy Key-In!");
        keyIn.Status = LeadStatus.Checkin;
        _context.Leads.Update(keyIn);
        await _context.LeadProcesses.AddAsync(new LeadProcess
        {
            LeadId = keyIn.Id,
            Status = LeadStatus.Checkin,
            CreatedDate = DateTime.Now,
            Note = $"Trả Key-In {keyIn.Name} ({keyIn.PhoneNumber}) về trạng thái checkin. Ghi chú: {args.Note}",
            UserId = _currentUser.GetId()
        });
        await _context.SaveChangesAsync();
        return TResult.Success;
    }

    public async Task<TResult> CreateAsync(EventCreateArgs args)
    {
        try
        {
            var room = await _roomService.FindAsync(args.RoomId);
            if (room is null) return TResult.Failed("Phòng không tồn tại!");
            await _eventRepository.AddAsync(new Event
            {
                Name = args.Name,
                StartDate = args.StartDate.Date.Add(args.StartTime),
                CreatedBy = _currentUser.GetId(),
                CreatedDate = DateTime.Now,
                Status = args.Status,
                RoomId = room.Id
            });
            return TResult.Success;
        }
        catch (Exception ex)
        {
            await _logService.ExceptionAsync(ex);
            return TResult.Failed(ex.ToString());
        }
    }

    public async Task<TResult> DeleteAsync(int id)
    {
        var data = await _eventRepository.FindAsync(id);
        if (data is null) return TResult.Failed("Sự kiện không tồn tại!");
        await _eventRepository.DeleteAsync(data);
        return TResult.Success;
    }

    public async Task<TResult> DeleteSaleRevenueAsync(Lead lead)
    {
        var topup = await _context.UserTopups.FirstOrDefaultAsync(x => x.LeadId == lead.Id);
        if (topup != null)
        {
            var topupEvidences = await _context.TopupEvidences.Where(x => x.TopupId == topup.Id).ToListAsync();
            if (topupEvidences != null)
            {
                foreach (var evidence in topupEvidences)
                {
                    _context.TopupEvidences.Remove(evidence);
                }
            }
            _context.Remove(topup);
            await _logService.AddAsync($"Xóa doanh thu sự kiện cho {topup.LeadId}");
        }
        return TResult.Success;
    }

    public Task<ListResult<object>> GetListAsync(EventFilterOptions filterOptions) => _eventRepository.GetListAsync(filterOptions);

    public async Task<ListResult<object>> ListKeyInRevenueAsync(SaleRevenueFilterOptions filterOptions)
    {
        var query = from a in _context.Leads
                    join c in _context.Users on a.SalesId equals c.Id
                    where a.Status == LeadStatus.Done || a.Status == LeadStatus.AccountantApproved || a.Status == LeadStatus.DosApproved || a.Status == LeadStatus.LeadAccept
                    select new
                    {
                        a.EventDate,
                        a.EventTime,
                        a.CreatedDate,
                        a.Status,
                        a.Note,
                        SaleName = c.Name,
                        SaleUserName = c.UserName,
                        KeyInId = a.Id,
                        KeyInName = a.Name,
                        KeyInPhoneNumber = a.PhoneNumber,
                        a.Branch,
                        Amount = _context.UserTopups.Where(x => x.LeadId == a.Id && x.Type == TopupType.Event && x.Status == TopupStatus.AccountantApproved).Sum(x => x.Amount),
                        AmountPending = _context.UserTopups.Where(x => x.LeadId == a.Id && x.Type == TopupType.Event && x.Status == TopupStatus.Pending).Sum(x => x.Amount)
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.KeyInName))
        {
            query = query.Where(x => x.KeyInName.ToLower().Contains(filterOptions.KeyInName.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.SaleName))
        {
            query = query.Where(x => x.SaleName.ToLower().Contains(filterOptions.SaleName.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.KeyInPhoneNumber))
        {
            query = query.Where(x => x.KeyInPhoneNumber.ToLower().Contains(filterOptions.KeyInPhoneNumber.ToLower()));
        }
        var user = await _userManager.FindByIdAsync(_currentUser.GetId().ToString());
        if (user is null) return ListResult<object>.Failed("User not found");
        query = query.Where(x => x.Branch == user.Branch);
        query = query.OrderByDescending(x => x.EventDate);
        return await ListResult<object>.Success(query, filterOptions);
    }

    public async Task<object?> ListSaleRevenueAsync(SaleRevenueFilterOptions filterOptions)
    {
        var query = from a in _context.UserTopups
                    join b in _context.Users on a.SaleId equals b.Id
                    join c in _context.Leads on a.LeadId equals c.Id
                    join d in _context.Users on a.CreatedBy equals d.Id
                    join e in _context.Users on a.AccountantId equals e.Id into e1
                    from e in e1.DefaultIfEmpty()
                    join f in _context.Users on a.DosId equals f.Id into f1
                    where a.Type == TopupType.Event
                    select new
                    {
                        a.Id,
                        a.Amount,
                        a.CreatedDate,
                        a.Status,
                        a.Note,
                        SaleName = b.Name,
                        SaleUserName = b.UserName,
                        KeyInName = c.Name,
                        KeyInPhoneNumber = c.PhoneNumber,
                        CreatedBy = d.Name,
                        c.Branch,
                        KeyInId = c.Id,
                        AccountantName = e.Name,
                        a.AccountantApprovedDate,
                        a.DirectorApprovedDate,
                    };
        var user = await _context.Users.FindAsync(_currentUser.GetId());
        if (user is null) return default;
        if (!string.IsNullOrWhiteSpace(filterOptions.SaleName))
        {
            query = query.Where(x => x.SaleName.ToLower().Contains(filterOptions.SaleName.ToLower()));
        }
        if (_currentUser.IsInRole(RoleName.Accountant) || _currentUser.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.Branch == user.Branch);
        }
        if (_currentUser.IsInRole(RoleName.Accountant))
        {
            query = query.Where(x => x.Status == TopupStatus.DirectorApproved);
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }

    public async Task<object?> RevenueHistoriesAsync(KeyInRevenueFilterOptions filterOptions)
    {
        var query = from a in _context.UserTopups
                    where a.Type == TopupType.Event && a.LeadId == filterOptions.KeyInId
                    select new
                    {
                        a.Id,
                        a.Amount,
                        a.CreatedDate,
                        a.Status,
                        a.Note,
                        SaleName = _context.Users.Where(x => x.Id == a.SaleId).Select(x => x.Name).FirstOrDefault(),
                        Creator = _context.Users.Where(x => x.Id == a.CreatedBy).Select(x => x.Name).FirstOrDefault(),
                        Evidences = _context.TopupEvidences.Where(x => x.TopupId == a.Id).Select(x => x.FileUrl).ToList(),
                        a.ContractCode
                    };
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }

    public async Task<TResult> UpdateAsync(EventUpdateArgs args)
    {
        var data = await _eventRepository.FindAsync(args.Id);
        if (data is null) return TResult.Failed("Sự kiện không tồn tại!");
        var room = await _roomService.FindAsync(args.RoomId);
        if (room is null) return TResult.Failed("Phòng không tồn tại!");
        data.Name = args.Name;
        data.ModifiedBy = _currentUser.GetId();
        data.ModifiedDate = DateTime.Now;
        data.StartDate = args.StartDate.Date.Add(args.StartTime);
        data.Status = args.Status;
        data.RoomId = room.Id;
        await _eventRepository.UpdateAsync(data);
        return TResult.Success;
    }

    public async Task<TResult> UpdateSaleRevenueAsync(AddSaleRevenue args)
    {
        var sale = await _context.Users.FindAsync(args.SaleId);
        if (sale is null) return TResult.Failed("Không tìm thấy trợ lý cá nhân!");
        if (sale.SmId is null) return TResult.Failed("Vui lòng chỉ định quản lý quan hệ khách hàng!");
        if (sale.DosId is null) return TResult.Failed("Vui lòng chỉ định giám đốc quan hệ khách hàng!");
        if (args.Amount <= 0) return TResult.Failed("Số tiền không hợp lệ!");
        var topup = await _context.UserTopups.FirstOrDefaultAsync(x => x.LeadId == args.LeadId);
        if (topup is null)
        {
            return await AddSaleRevenueAsync(args);
        }
        topup.Amount = args.Amount;
        topup.SaleId = args.SaleId;
        topup.Status = TopupStatus.Pending;
        topup.ModifiedDate = DateTime.Now;
        topup.SmId = sale.SmId;
        topup.DosId = sale.DosId;
        topup.ModifiedBy = _currentUser.GetId();
        topup.Note = args.Note;
        topup.ContractCode = args.ContractCode;
        _context.UserTopups.Update(topup);
        await _logService.AddAsync($"Cập nhật doanh thu sự kiện cho {sale.Name} ({sale.UserName})");
        return TResult.Success;
    }
}
