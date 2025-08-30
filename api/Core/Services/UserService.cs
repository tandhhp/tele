using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Waffle.Core.Constants;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IService;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Entities.Contacts;
using Waffle.ExternalAPI;
using Waffle.Models;
using Waffle.Models.Filters;
using Waffle.Models.Users;
using Waffle.Models.ViewModels.Users;

namespace Waffle.Core.Services;

public class UserService(UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager, ApplicationDbContext _context, ICurrentUser currentUser) : IUserService
{
    private readonly ICurrentUser _currentUser = currentUser;

    private async Task<ApplicationUser?> FindAsync(Guid id) => await _context.Users.FindAsync(id);

    public async Task<IdentityResult> AddToRoleAsync(AddToRoleModel model)
    {
        var user = await FindAsync(model.Id);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "User not found!"
            });
        }
        if (!await _roleManager.RoleExistsAsync(model.RoleName))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Role not found!"
            });
        }
        return await _userManager.AddToRoleAsync(user, model.RoleName);
    }

    public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel args)
    {
        var user = await FindAsync(args.Id);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found!"
            });
        }
        return await _userManager.ChangePasswordAsync(user, args.CurrentPassword, args.NewPassword);
    }

    public async Task<IdentityResult> CreateAsync(CreateUserModel model)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(x => x.Id == model.CardId);
        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.ContractCode,
            PhoneNumber = model.PhoneNumber,
            Address = model.Address,
            Avatar = model.Avatar,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Loyalty = 0,
            Name = model.Name,
            CardId = model.CardId,
            CreatedDate = DateTime.Now,
            Concerns = model.Concerns,
            ContractCode = model.ContractCode,
            FamilyCharacteristics = model.FamilyCharacteristics,
            IdentityAddress = model.IdentityAddress,
            IdentityDate = model.IdentityDate,
            IdentityNumber = model.IdentityNumber,
            HealthHistory = model.HealthHistory,
            Personality = model.Personality,
            LoyaltyExpiredDate = model.ContractDate.GetValueOrDefault().AddYears(1),
            SellerId = model.SallerId,
            MaxLoyalty = model.MaxLoyalty ?? card?.Loyalty ?? 0,
            HasChange = false,
            ContractDate = model.ContractDate,
            Branch = model.Branch
        };
        var lead = new Lead();
        if (model.LeadConvert)
        {
            lead = await _context.Leads.FindAsync(model.LeadId);
            if (lead is null) return IdentityResult.Failed(new IdentityError
            {
                Code = HttpStatusCode.NotFound.ToString(),
                Description = "Không tìm thấy khách hàng tiềm năng!"
            });
            user.SellerId = lead.SalesId;
        }
        if (model.SallerId != null)
        {
            var sales = await _userManager.FindByIdAsync(model.SallerId.GetValueOrDefault().ToString());
            if (sales is null) return IdentityResult.Failed(new IdentityError
            {
                Code = HttpStatusCode.NotFound.ToString(),
                Description = "Không tìm thấy trợ lý cá nhân!"
            });
            user.SmId = sales.SmId;
            user.DosId = sales.DosId;
            user.Branch = sales.Branch;
        }
        model.Password = $"nuras@{user.UserName}";
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            var role = await _roleManager.FindByNameAsync(RoleName.CardHolder);
            if (role is null)
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    DisplayName = "Chủ thẻ",
                    Name = RoleName.CardHolder
                });
            }
            await _userManager.AddToRoleAsync(user, RoleName.CardHolder);

            // Nếu là lead convert sang thì set trạng thái lead này là hoàn thành và chuyển cho cx quản lý
            if (model.LeadConvert)
            {
                if (lead is null)  return IdentityResult.Failed(new IdentityError
                {
                    Code = HttpStatusCode.NotFound.ToString(),
                    Description = "Không tìm thấy khách hàng tiềm năng!"
                });

                lead.Status = LeadStatus.Done;
                _context.Leads.Update(lead);
                var leadFeedback = await _context.LeadFeedbacks.FirstOrDefaultAsync(x => x.LeadId == lead.Id);
                if (leadFeedback is null) return IdentityResult.Failed(new IdentityError
                {
                    Code = HttpStatusCode.NotFound.ToString(),
                    Description = "Không tìm thấy feedback!"
                });

                // Nếu lead này đã có công nợ thì không tạo mới
                if (!await _context.UserTopups.AnyAsync(x => x.LeadId == lead.Id))
                {
                    await _context.UserTopups.AddAsync(new UserTopup
                    {
                        CreatedDate = DateTime.Now,
                        CardHolderId = user.Id,
                        DirectorApprovedDate = DateTime.Now,
                        DirectorId = lead.DosId,
                        AccountantId = lead.AccountantId,
                        Amount = leadFeedback.AmountPaid ?? 0,
                        DosId = lead.DosId,
                        Note = "Tự động tạo công nợ theo sự kiện",
                        AccountantApprovedDate = DateTime.Now,
                        Status = TopupStatus.AccountantApproved,
                        SaleId = user.SellerId ?? Guid.Empty,
                        SmId = user.SmId,
                        Type = TopupType.Debt,
                        LeadId = lead.Id,
                        ContractCode = leadFeedback.ContractCode
                    });
                }

                await _context.SaveChangesAsync();
            }

            await SendEmailToCardHolderAsync(new CreateUserModel
            {
                Name = user.Name,
                UserName = user.UserName,
                Password = model.Password,
                Email = user.Email
            });
        }
        return result;
    }

    public async Task SendEmailToCardHolderAsync(CreateUserModel user)
    {
        var subject = "[NURA'S] Thông báo kích hoạt tài khoản thành công";
        var body = $@"
<i>Kính chào <b>Quý Chủ Thẻ {user.Name}</b></i> <br/>
<i><b>Thân mến,</b></i> <br/>
Lời đầu tiên, <b>NURA'S</b> xin gửi lời chào đến <b>Quý Chủ Thẻ {user.Name}</b>, <b>NURA'S</b> chúc mừng <b>Quý Chủ Thẻ {user.Name}</b> đã kích hoạt thành công tài khoản. Thông tin đăng nhập và hướng dẫn sử dụng như sau:<br/>
<ul>
<li>ID đăng nhập: <b>{user.UserName}</b></li>
<li>Mật khẩu: <b>{user.Password}</b></li>
<li>Đăng nhập tại: <b>https://me.nuras.com.vn</b></li>
</ul><br/>
Nhằm mang lại những trải nghiệm trọn vẹn nhất cho <b>Quý Chủ Thẻ {user.Name}</b>, với đội ngũ <b>Trợ Lý Cá Nhân và Bộ phận Trải Nghiệm Khách Hàng</b> chuyên nghiệp. Chúng tôi sẽ luôn đảm bảo hỗ trợ khách hàng một cách tận tâm nhất.<br/>
Thông tin liên hệ của Bộ phận <b><i>Trải Nghiệm Khách Hàng</i></b>:<br/>
<ul>
<li><b>Hotline:</b> 099.661.5028</li>
<li><b>Email:</b> cskh@nuras.com.vn</li>
<li><b>Giờ làm việc:</b> Thứ 2 - Chủ Nhật (7h30-17h30).</li>
</ul><br/>
<b>NURA'S</b> kính chúc <b>Quý Chủ Thẻ {user.Name}</b> những điều tuyệt vời nhất và ước mong luôn được đồng hành cùng <b>Quý Chủ Thẻ và Gia Đình</b> thân yêu trên hành trình hạnh phúc này.<br/>
<b>Trân trọng!</b>
";
        if (!string.IsNullOrEmpty(user.Email))
        {
            await Sender.SendAsync(user.Email, subject, body);
        }
    }

    public async Task<CurrentUserViewModel?> GetCurrentUserAsync(Guid id)
    {
        var user = await FindAsync(id);
        if (user is null) return default;
        var card = await _context.Cards.FindAsync(user.CardId ?? Guid.Empty);
        var seller = string.Empty;
        if (user.SellerId != null)
        {
            var sellerdb = await _context.Users.FindAsync(user.SellerId);
            seller = $"{sellerdb?.Name} ({sellerdb?.UserName})";
        }
        if (user.ContractDate != null)
        {
            if (user.LoyaltyExpiredDate == null)
            {
                user.LoyaltyExpiredDate = user.ContractDate.GetValueOrDefault().AddYears(1);
            }
            else
            {
                if (user.ContractDate.GetValueOrDefault().Date.AddYears(1) > user.LoyaltyExpiredDate.GetValueOrDefault().Date)
                {
                    user.LoyaltyExpiredDate = user.ContractDate.GetValueOrDefault().AddYears(1);
                }
                if (user.LoyaltyExpiredDate.GetValueOrDefault().Date == DateTime.Today)
                {
                    user.Loyalty = 0;
                    user.LoyaltyExpiredDate = DateTime.Today.AddYears(1);
                }
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        var subContracts = await _context.Contracts.Where(x => x.CardHolderId == user.Id).ToListAsync();

        return new CurrentUserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName,
            Name = user.Name,
            Roles = await _userManager.GetRolesAsync(user),
            Loyalty = user.Loyalty,
            Avatar = user.Avatar,
            Gender = user.Gender,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Tier = card?.Tier ?? Tier.Standard,
            CardId = user.CardId,
            Concerns = user.Concerns,
            ContractCode = user.ContractCode,
            CreatedDate = user.CreatedDate,
            Token = user.Token,
            CardBackImage = card?.BackImage,
            CardFrontImage = card?.FrontImage,
            LoyaltyExpiredDate = user.LoyaltyExpiredDate,
            Seller = seller,
            FamilyCharacteristics = user.FamilyCharacteristics,
            HealthHistory = user.HealthHistory,
            IdentityAddress = user.IdentityAddress,
            IdentityNumber = user.IdentityNumber,
            IdentityDate = user.IdentityDate,
            SellerId = user.SellerId,
            Personality = user.Personality,
            LoanPoint = user.LoanPoint,
            Amount = user.Amount,
            MaxLoyalty = user.MaxLoyalty,
            ContractDate = user.ContractDate,
            Branch = user.Branch,
            SubContracts = subContracts,
            TierColor = card?.Color,
            TierName = card?.Code
        };
    }

    public async Task<dynamic> GetUsersInRoleAsync(string roleName, UserFilterOptions filterOptions)
    {
        var data = await _userManager.GetUsersInRoleAsync(roleName);
        data = data.Where(x => x.HasChange != true).ToList();
        if (filterOptions.Status != null)
        {
            data = data.Where(x => x.Status == filterOptions.Status).ToList();
        }
        if (filterOptions.SmId != null)
        {
            data = data.Where(x => x.SmId == filterOptions.SmId).ToList();
        }
        if (filterOptions.TmId != null)
        {
            data = data.Where(x => x.TmId == filterOptions.TmId).ToList();
        }
        var user = await _userManager.FindByIdAsync(_currentUser.GetId().ToString());
        if (user is null) return default!;
        if (!filterOptions.IsAdmin)
        {
            data = data.Where(x => x.Branch == user.Branch).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.UserName))
        {
            data = data.Where(x => x.UserName != null && x.UserName.ToLower().Contains(filterOptions.UserName.ToLower())).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            data = data.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(filterOptions.Name.ToLower())).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            data = data.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Email))
        {
            data = data.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(filterOptions.Email.ToLower())).ToList();
        }
        return new
        {
            data,
            total = data.Count,
        };
    }

    public async Task<ListResult<dynamic>> ListContactAsync(ContactFilterOptions filterOptions)
    {
        var query = from a in _context.Contacts
                    where a.Status != ContactStatus.Blacklisted
                    select new
                    {
                        a.Id,
                        a.PhoneNumber,
                        a.Email,
                        a.CreatedDate,
                        a.Name,
                        a.Note
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Email))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(filterOptions.Email.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return await ListResult<dynamic>.Success(query, filterOptions);
    }

    public async Task<IdentityResult> RemoveFromRoleAsync(RemoveFromRoleModel args)
    {
        var user = await FindAsync(args.Id);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "User not found!"
            });
        }
        return await _userManager.RemoveFromRoleAsync(user, args.RoleName);
    }

    public async Task<ListResult<object>> PointsAsync(UserPointFilterOptions filterOptions)
    {
        var query = from a in _context.UserPoints
                    join b in _context.Users on a.CreatedBy equals b.Id
                    where a.CardHolderId == filterOptions.CardHolderId
                    select new
                    {
                        a.Id,
                        a.Point,
                        a.CreatedDate,
                        a.DueDate,
                        TopupBy = b.Name
                    };
        return await ListResult<object>.Success(query, filterOptions);
    }

    public async Task<TResult> CreateAsync(CreateUserArgs args)
    {
        await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = args.UserName,
            Email = args.Email,
            CreatedDate = DateTime.Now,
            Status = UserStatus.Working,
            Name = args.Name,
            PhoneNumber = args.PhoneNumber
        }, args.Password);
        return TResult.Success;
    }
}
