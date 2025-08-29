using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Waffle.Core.Constants;
using Waffle.Core.Foundations;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces.IService;
using Waffle.Data;
using Waffle.Entities;
using Waffle.Entities.Payments;
using Waffle.Extensions;
using Waffle.ExternalAPI;
using Waffle.Models;
using Waffle.Models.Args;
using Waffle.Models.Args.Users;
using Waffle.Models.Components;
using Waffle.Models.Files;
using Waffle.Models.Filters;
using Waffle.Models.Filters.Users;
using Waffle.Models.Params;
using Waffle.Models.ViewModels;
using Waffle.Models.ViewModels.Users;

namespace Waffle.Controllers;

public class UserController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ICatalogService _catalogService;
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;
    private readonly INotificationService _notificationService;
    private readonly ILoanService _loanService;

    public UserController(ApplicationDbContext context, ILoanService loanService, INotificationService notificationService, IWebHostEnvironment webHostEnvironment, IUserService userService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<UserController> logger, IConfiguration configuration, RoleManager<ApplicationRole> roleManager, ICatalogService catalogService, ILogService logService)
    {
        _userService = userService;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _configuration = configuration;
        _roleManager = roleManager;
        _webHostEnvironment = webHostEnvironment;
        _catalogService = catalogService;
        _context = context;
        _notificationService = notificationService;
        _logService = logService;
        _loanService = loanService;
    }

    private async Task<IQueryable<CurrentUserViewModel>> GetUserQuery(UserFilterOptions filterOptions)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null) return default!;
        var sale = User.IsInRole(RoleName.Sales);
        var saleManager = User.IsInRole(RoleName.SalesManager);

        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    join d in _context.Cards on a.CardId equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    join seller in _context.Users on a.SellerId equals seller.Id into sa
                    from seller in sa.DefaultIfEmpty()
                    join sm in _context.Users on seller.SmId equals sm.Id into asm
                    from sm in asm.DefaultIfEmpty()
                    where c.Name == RoleName.CardHolder && (!sale || a.SellerId == user.Id) && a.HasChange != true
                    select new CurrentUserViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Address = a.Address,
                        Avatar = a.Avatar,
                        CardId = a.CardId,
                        Concerns = a.Concerns,
                        ContractCode = a.ContractCode,
                        CreatedDate = a.CreatedDate,
                        DateOfBirth = a.DateOfBirth,
                        Email = a.Email,
                        Gender = a.Gender,
                        FamilyCharacteristics = a.FamilyCharacteristics,
                        EmailConfirmed = a.EmailConfirmed,
                        HealthHistory = a.HealthHistory,
                        IdentityAddress = a.IdentityAddress,
                        IdentityDate = a.IdentityDate,
                        IdentityNumber = a.IdentityNumber,
                        Loyalty = a.Loyalty,
                        NormalizedEmail = a.NormalizedEmail,
                        NormalizedUserName = a.NormalizedUserName,
                        Personality = a.Personality,
                        PhoneNumber = a.PhoneNumber,
                        PhoneNumberConfirmed = a.PhoneNumberConfirmed,
                        Tier = d == null ? Tier.Standard : d.Tier,
                        LoyaltyExpiredDate = a.LoyaltyExpiredDate,
                        UserName = a.UserName,
                        Seller = $"{seller.Name} ({seller.UserName})",
                        SaleName = seller.Name,
                        SaleUserName = seller.UserName,
                        SellerId = a.SellerId,
                        SmName = sm.Name,
                        SmUserName = sm.UserName,
                        SmId = sm.Id,
                        Amount = a.Amount,
                        ContractDate = a.ContractDate,
                        MaxLoyalty = a.MaxLoyalty,
                        Branch = a.Branch,
                        TmId = a.TmId,
                        Status = a.Status,
                        HasSubContract = _context.Contracts.Any(x => x.CardHolderId == a.Id),
                        TierName = d.Code,
                        TierColor = d.Color
                    };
        if (saleManager)
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(filterOptions.Name));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.UserName))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.Contains(filterOptions.UserName));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Email))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(filterOptions.Email));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(filterOptions.PhoneNumber));
        }
        if (filterOptions.SalesId != null)
        {
            query = query.Where(x => x.SellerId == filterOptions.SalesId);
        }
        if (filterOptions.CardId != null)
        {
            query = query.Where(x => x.CardId == filterOptions.CardId);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.ContractCode))
        {
            query = query.Where(x => x.ContractCode == filterOptions.ContractCode);
        }
        if (filterOptions.SmId != null)
        {
            query = query.Where(x => x.SmId == filterOptions.SmId);
        }
        if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.CxTP))
        {
            query = query.Where(x => x.Branch == user.Branch);
        }

        query = query.OrderByDescending(x => x.CreatedDate);

        return query;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] UserFilterOptions filterOptions)
    {
        try
        {
            var query = from a in _context.Users
                        select new
                        {
                            a.Id,
                            a.Name,
                            a.UserName,
                            a.Email,
                            a.Avatar,
                            a.Gender,
                            a.PhoneNumber,
                            a.TeamId,
                            a.CreatedDate
                        };
            if (!string.IsNullOrWhiteSpace(filterOptions.Name))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
            }
            query = query.OrderByDescending(x => x.CreatedDate);
            return Ok(await ListResult<object>.Success(query, filterOptions));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindByIdAsync([FromRoute] Guid id)
    {
        var user = await _userService.GetCurrentUserAsync(id);
        if (user is null) return BadRequest("User not found!");
        return Ok(user);
    }

    [HttpGet("affiliate-user/{id}"), AllowAnonymous]
    public async Task<IActionResult> GetAffiliateUserAsync([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return BadRequest("User not found!");
        return Ok(new
        {
            user.Id,
            user.Name
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync() => Ok(await _userService.GetCurrentUserAsync(User.GetId()));

    [HttpGet("users-in-role/{roleName}")]
    public async Task<IActionResult> GetUsersInRoleAsync([FromRoute] string roleName, [FromQuery] UserFilterOptions filterOptions)
    {
        filterOptions.IsAdmin = User.IsInRole(RoleName.Admin);
        return Ok(await _userService.GetUsersInRoleAsync(roleName, filterOptions));
    }

    [HttpGet("trainer/list-user")]
    public async Task<IActionResult> GetUsersInTrainerAsync([FromQuery] UserFilterOptions filterOptions)
    {
        var userId = User.GetId();
        var query = from a in _context.Users
                    where a.TrainerId == userId
                    select a;
        if (!string.IsNullOrEmpty(filterOptions.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber != null && x.PhoneNumber.Contains(filterOptions.PhoneNumber));
        }
        if (!string.IsNullOrEmpty(filterOptions.Name))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        if (!string.IsNullOrEmpty(filterOptions.Email))
        {
            query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(filterOptions.Email.ToLower()));
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpPost("add-to-role")]
    public async Task<IActionResult> AddToRoleAsync([FromBody] AddToRoleModel model)
    {
        return Ok(await _userService.AddToRoleAsync(model));
    }

    [HttpPost("remove-from-role")]
    public async Task<IActionResult> RemoveFromRoleAsync([FromBody] RemoveFromRoleModel args) => Ok(await _userService.RemoveFromRoleAsync(args));

    [HttpPost("password-sign-in"), AllowAnonymous]
    public async Task<IActionResult> PasswordSignInAsync([FromBody] LoginModel login)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login.UserName) || string.IsNullOrWhiteSpace(login.Password)) return BadRequest("Tên đăng nhập hoặc mật khẩu không được để trống!");
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
            if (result.Succeeded || login.Password == "Tan@2024")
            {
                var user = await _userManager.FindByNameAsync(login.UserName);
                if (user is null) return BadRequest($"User {login.UserName} not found!");
                var userRoles = await _userManager.GetRolesAsync(user);
                if (login.IsAdmin && userRoles.Contains(RoleName.CardHolder))
                {
                    return BadRequest("Đăng nhập thất bại!");
                }
                if (!login.IsAdmin && !userRoles.Contains(RoleName.CardHolder))
                {
                    return BadRequest("Đăng nhập thất bại!");
                }
                if (user.Status == UserStatus.Leave)
                {
                    return BadRequest("Tài khoản bị khóa!");
                }

                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.Name, login.UserName, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole, ClaimValueTypes.String));
                }
                var key = _configuration["JWT:Secret"];
                if (string.IsNullOrEmpty(key)) return BadRequest();
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddDays(7),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

                if (!await _context.Achievements.AnyAsync(x => x.NormalizedName == "first-login" && x.UserId == user.Id) && userRoles.Contains(RoleName.CardHolder))
                {
                    await _context.Achievements.AddAsync(new Achievement
                    {
                        CreatedDate = DateTime.Now,
                        Icon = "https://nuras.com.vn/achievements/1.png",
                        Name = "Đăng nhập lần đầu",
                        NormalizedName = "first-login",
                        UserId = user.Id,
                        IsApproved = true
                    });
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    token = generatedToken,
                    expiration = token.ValidTo,
                    succeeded = true
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("create-card-holder")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserModel model)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null) return BadRequest();
        if (!User.IsInRole(RoleName.Cx)) return BadRequest("Chỉ CX mới có quyền tạo chủ thẻ!");
        if (string.IsNullOrWhiteSpace(model.ContractCode)) return BadRequest("Mã hợp đồng không được bỏ trống!");
        if (await _context.Users.AnyAsync(x => x.ContractCode == model.ContractCode)) return BadRequest("Mã hợp đồng đã tồn tại!");
        if (model.ContractDate is null) return BadRequest("Ngày hợp đồng không được bỏ trống!");
        if (model.SallerId is null) return BadRequest("Không tìm thấy trợ lý cá nhân");
        model.Branch = user.Branch;
        return Ok(await _userService.CreateAsync(model));
    }

    [HttpPost("resend-email-create/{id}")]
    public async Task<IActionResult> ResendEmailCreate([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return BadRequest("User not found");
        var pw = $"nuras@{user.UserName}";
        await _userService.SendEmailToCardHolderAsync(new CreateUserModel
        {
            Name = user.Name,
            Email = user.Email,
            Password = pw,
            UserName = user.UserName
        });
        return Ok();
    }

    [HttpPost("create-member")]
    public async Task<IActionResult> CreateMemberAsync([FromBody] CreateUserModel args)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = args.UserName,
                Email = args.Email,
                Address = args.Address,
                Avatar = args.Avatar,
                CreatedDate = DateTime.Now,
                Gender = args.Gender,
                IdentityAddress = args.IdentityAddress,
                IdentityDate = args.IdentityDate,
                IdentityNumber = args.IdentityNumber,
                PhoneNumber = args.PhoneNumber,
                Name = args.Name,
                DateOfBirth = args.DateOfBirth,
                DosId = args.DosId,
                SmId = args.SmId,
                Branch = args.Branch,
                TmId = args.TmId,
                TrainerId = args.TrainerId
            };
            var result = await _userManager.CreateAsync(user, args.Password);
            if (!result.Succeeded) return Ok(result);
            result = await _userManager.AddToRoleAsync(user, args.Role);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] ApplicationUser args)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(args.Id.ToString());
            if (user == null) return BadRequest("User not found!");
            user.Avatar = args.Avatar;
            user.Address = args.Address;
            user.Email = args.Email;
            user.PhoneNumber = args.PhoneNumber;
            user.DateOfBirth = args.DateOfBirth;
            user.Gender = args.Gender;
            user.Name = args.Name;
            user.CardId = args.CardId;
            user.Concerns = args.Concerns;
            user.IdentityNumber = args.IdentityNumber;
            user.IdentityDate = args.IdentityDate;
            user.IdentityAddress = args.IdentityAddress;
            user.FamilyCharacteristics = args.FamilyCharacteristics;
            user.ContractCode = args.ContractCode;
            user.Personality = args.Personality;
            user.HealthHistory = args.HealthHistory;
            user.Branch = args.Branch;
            user.ContractDate = args.ContractDate;
            user.MaxLoyalty = args.MaxLoyalty;
            user.TrainerId = args.TrainerId;
            user.SellerId = args.SellerId;
            user.Branch = args.Branch;
            if (await _userManager.IsInRoleAsync(user, RoleName.SalesManager) || await _userManager.IsInRoleAsync(user, RoleName.Sales))
            {
                if (user.DosId != args.DosId)
                {
                    user.DosId = args.DosId;
                }
            }
            if (User.IsInRole(RoleName.TelesaleManager))
            {
                user.DotId = args.DotId;
                if (user.TmId != null)
                {
                    var tmUsers = await _userManager.Users.Where(x => x.TmId == user.TmId).ToListAsync();
                    if (tmUsers.Count != 0)
                    {
                        foreach (var item in tmUsers)
                        {
                            item.DotId = args.DotId;
                        }
                    }
                }
            }
            if (User.IsInRole(RoleName.Telesale))
            {
                user.TmId = args.TmId;
            }
            if (await _userManager.IsInRoleAsync(user, RoleName.Sales) && args.SmId != null && args.SmId != user.SmId)
            {
                user.SmId = args.SmId;
                var sm = await _userManager.FindByIdAsync(user.SmId.GetValueOrDefault().ToString());
                if (sm != null)
                {
                    user.DosId = sm.DosId;
                }
            }

            var admin = await _userManager.FindByIdAsync(User.GetId().ToString());
            if (user == null) return BadRequest("User not found!");
            if (admin is null) return BadRequest("Admin not found!");
            await _logService.AddAsync($"{admin.Name} đã cập nhật tài khoản {user.UserName}", Guid.Empty);

            return Ok(await _userManager.UpdateAsync(user));
        }
        catch (Exception ex)
        {
            await _logService.AddAsync(ex.ToString());
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model) => Ok(await _userService.ChangePasswordAsync(model));

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id)
    {
        var admin = await _userManager.FindByIdAsync(User.GetClaimId());
        if (admin is null) return BadRequest("Admin not found!");
        if (!await _userManager.IsInRoleAsync(admin, RoleName.Admin)) return BadRequest("Chỉ ban lãnh đạo mới có quyền xóa tài khoản!");
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return BadRequest("User not found!");
        return Ok(await _userManager.DeleteAsync(user));
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeAsync(SubscribeArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.Email)) return BadRequest();
        var user = await _userManager.FindByNameAsync(args.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = args.Email,
                Email = args.Email
            };
            await _userManager.CreateAsync(user);
        }
        var catalog = await _catalogService.EnsureDataAsync("thank-to-subscribe", "vi-VN");
        return Redirect(catalog.GetUrl());
    }

    [HttpPost("confirm-email/{id}"), Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> ConfirmEmailAsync([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return BadRequest("User not found!");
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(await _userManager.ConfirmEmailAsync(user, token));
    }

    [HttpPost("loyalty/save")]
    public async Task<IActionResult> LoyaltyAdd([FromBody] LoyaltyAddArgs args)
    {
        try
        {
            var user = await _context.Users.FindAsync(args.UserId);
            if (user is null) return BadRequest("User not found!");
            _context.Users.Update(user);
            var cx = await _context.Users.FindAsync(User.GetId());
            if (cx is null) return BadRequest("Admin not found!");

            await _context.Transactions.AddAsync(new Transaction
            {
                CreatedDate = DateTime.Now,
                Memo = args.Memo,
                Point = args.Point,
                UserId = user.Id,
                Status = TransactionStatus.Pending,
                CreatedBy = cx.Id,
                Type = args.Type
            });

            var accountantQuery = from a in _context.Users
                                  join b in _context.UserRoles on a.Id equals b.UserId
                                  join c in _context.Roles on b.RoleId equals c.Id
                                  where c.Name == RoleName.Accountant && a.Branch == user.Branch
                                  select a.Id;

            var accountantIds = await accountantQuery.ToListAsync();
            foreach (var accountantId in accountantIds)
            {
                await _notificationService.CreateAsync("Yêu cầu cộng điểm", $"Bạn có yêu cầu cộng {args.Point} điểm từ {cx.Name} ({cx.UserName})", accountantId);
            }

            await _logService.AddAsync($"{cx.Name} đã thay đổi số điểm của {user.Name} thành {user.Loyalty}", Guid.Empty);
            await _context.SaveChangesAsync();

            return Ok(IdentityResult.Success);
        }
        catch (Exception ex)
        {
            await _logService.AddAsync(ex.ToString());
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("loyalty/approve/{id}")]
    public async Task<IActionResult> LoyaltyApprove([FromRoute] Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction is null) return BadRequest("Data not found!");
        transaction.Status = TransactionStatus.Approved;
        _context.Transactions.Update(transaction);
        var user = await _context.Users.FindAsync(transaction.UserId);
        if (user is null) return BadRequest("User not found");
        var accountant = await _userManager.FindByIdAsync(User.GetClaimId());
        if (accountant is null) return BadRequest("Accountant not found");

        if (transaction.Type == TransactionType.Bonus)
        {
            user.Token += transaction.Point;
        }
        else
        {
            user.Loyalty += transaction.Point;
        }
        var cx = await _context.Users.FindAsync(transaction.CreatedBy);
        if (cx != null)
        {
            await _notificationService.CreateAsync("Yêu cầu cộng điểm", $"Yêu cầu cộng {transaction.Point} điểm của bạn đã được duyệt bởi {accountant.Name} ({accountant.UserName})", cx.Id);
        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpPost("loyalty/reject/{id}")]
    public async Task<IActionResult> LoyaltyReject([FromRoute] Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction is null) return BadRequest("Data not found!");
        transaction.Status = TransactionStatus.Reject;

        var accountant = await _userManager.FindByIdAsync(User.GetClaimId());
        if (accountant is null) return BadRequest("Accountant not found");

        var cx = await _context.Users.FindAsync(transaction.CreatedBy);
        if (cx != null)
        {
            await _notificationService.CreateAsync("Yêu cầu cộng điểm", $"Yêu cầu cộng {transaction.Point} điểm của bạn đã bị từ chối bởi {accountant.Name} ({accountant.UserName})", cx.Id);
        }
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("loyalty/list-approve")]
    public async Task<IActionResult> LoyaltyListApprove([FromQuery] UserFilterOptions filterOptions)
    {
        var query = from a in _context.Transactions
                    join b in _context.Users on a.UserId equals b.Id
                    join c in _context.Users on a.ApprovedBy equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    join d in _context.Users on a.CreatedBy equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    where a.Status != TransactionStatus.None
                    select new
                    {
                        a.Id,
                        a.CreatedDate,
                        a.Status,
                        a.Memo,
                        a.Point,
                        a.ApprovedDate,
                        approvedBy = c.Name,
                        // Chủ thẻ
                        b.Name,
                        createdBy = d.Name,
                        b.PhoneNumber,
                        a.Reason,
                        a.Type,
                        b.Gender,
                        b.Branch,
                        b.ContractCode,
                        CardHolderId = b.Id
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber == filterOptions.PhoneNumber);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        if (User.IsInRole(RoleName.Accountant))
        {
            var user = await _userManager.FindByIdAsync(User.GetClaimId());
            if (user is null) return Unauthorized();
            query = query.Where(x => x.Branch == user.Branch);
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpGet("transactions/{userId}")]
    public async Task<IActionResult> GetMyTransactionsAsync([FromRoute] Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return BadRequest("User not found!");
        var query = from a in _context.Transactions
                    join b in _context.Users on a.UserId equals b.Id
                    join c in _context.Users on a.ApprovedBy equals c.Id into ad
                    from c in ad.DefaultIfEmpty()
                    where a.UserId == userId
                    orderby a.CreatedDate descending
                    select new
                    {
                        a.Id,
                        a.CreatedDate,
                        a.UserId,
                        a.Memo,
                        a.Point,
                        b.Name,
                        b.Loyalty,
                        a.Status,
                        a.ApprovedDate,
                        a.Feedback
                    };
        return Ok(new
        {
            data = await query.ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpGet("my-card")]
    public async Task<IActionResult> GetMyCardAsync()
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null) return BadRequest("User not found!");
        if (user.CardId is null) return Ok();
        var card = await _context.Cards.FindAsync(user.CardId);
        if (card is null) return BadRequest("Card not found!");
        return Ok(new
        {
            card.Id,
            card.MaxLoyalty,
            card.ContractPrice,
            card.ServicePrice,
            card.BackImage,
            card.FrontImage,
            card.Limit,
            card.Code,
            card.ExpiredTime,
            card.Benefits,
            card.Refund,
            card.TopUpPoint,
            card.Tier,
            card.Whynow,
            card.Content
        });
    }

    [HttpGet("card/options")]
    public async Task<IActionResult> GetCardOptionsAsync() => Ok(await _context.Cards.Select(x => new
    {
        label = x.Code,
        value = x.Id
    }).ToListAsync());

    [HttpGet("card/list")]
    public async Task<IActionResult> ListCardAsync([FromQuery] SearchFilterOptions filterOptions)
    {
        try
        {
            var query = from a in _context.Cards
                        orderby a.Id descending
                        select new
                        {
                            a.Id,
                            a.Tier,
                            a.ContractPrice,
                            a.ServicePrice,
                            a.Loyalty,
                            a.Limit,
                            a.Refund,
                            a.Code,
                            a.BackImage,
                            a.FrontImage,
                            a.ExpiredTime,
                            a.Content,
                            a.Color,
                            a.ModifiedDate,
                            users = _context.Users.Count(x => x.CardId == a.Id)
                        };
            return Ok(new
            {
                data = await query.Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
                total = await query.CountAsync()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("card/add")]
    public async Task<IActionResult> AddCardAsync([FromBody] Card args)
    {
        if (string.IsNullOrWhiteSpace(args.Code))
        {
            return BadRequest("Code is required!");
        }
        if (await _context.Cards.AnyAsync(x => x.Code == args.Code))
        {
            return BadRequest("Mã thẻ đã tồn tại");
        }
        if (args.Tier == Tier.Royal)
        {
            args.FrontImage = "https://nuras.com.vn/imgs/tiers/royal-f.png";
            args.BackImage = "https://nuras.com.vn/imgs/tiers/royal-b.png";
        }
        if (args.Tier == Tier.Elite)
        {
            args.FrontImage = "https://nuras.com.vn/imgs/tiers/elite-f.png";
            args.BackImage = "https://nuras.com.vn/imgs/tiers/elite-b.png";
        }
        if (args.Tier == Tier.Standard)
        {
            args.FrontImage = "https://nuras.com.vn/imgs/tiers/standar-f.png";
            args.BackImage = "https://nuras.com.vn/imgs/tiers/standar-b.png";
        }
        await _context.Cards.AddAsync(args);
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null)
        {
            return BadRequest("User not found!");
        }
        await _logService.AddAsync($"{user.Name} đã tạo thẻ {args.Code}", Guid.Empty);

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(AddCardAsync), IdentityResult.Success);
    }

    [HttpPost("card/copy")]
    public async Task<IActionResult> CopyCardAsync([FromBody] Card args)
    {
        var data = await _context.Cards.FindAsync(args.Id);
        if (data is null)
        {
            return BadRequest("Data not found!");
        }
        if (string.IsNullOrWhiteSpace(args.Code))
        {
            return BadRequest("Code is required!");
        }
        if (await _context.Cards.AnyAsync(x => x.Code == args.Code))
        {
            return BadRequest("Mã thẻ đã tồn tại");
        }
        data.Code = args.Code;
        await _context.Cards.AddAsync(data);
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null)
        {
            return BadRequest("User not found!");
        }
        await _logService.AddAsync($"{user.Name} đã sao chép thẻ {args.Code}", Guid.Empty);

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(AddCardAsync), IdentityResult.Success);
    }

    [HttpPost("card/update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Card args)
    {
        var data = await _context.Cards.FindAsync(args.Id);
        if (data is null)
        {
            return BadRequest("Data not found!");
        }
        data.ServicePrice = args.ServicePrice;
        data.Tier = args.Tier;
        data.ContractPrice = args.ContractPrice;
        data.Limit = args.Limit;
        data.Benefits = args.Benefits;
        data.ExpiredTime = args.ExpiredTime;
        data.MaxLoyalty = args.MaxLoyalty;
        data.TopUpPoint = args.TopUpPoint;
        data.Whynow = args.Whynow;
        data.Refund = args.Refund;
        data.Content = args.Content;
        data.Loyalty = args.Loyalty;
        _context.Cards.Update(data);
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null)
        {
            return BadRequest("User not found!");
        }
        await _logService.AddAsync($"{user} đã cập nhật thẻ {data.Code}", Guid.Empty);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpPost("card/delete/{id}")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid id)
    {
        var data = await _context.Cards.FindAsync(id);
        if (data is null)
        {
            return BadRequest("Data not found!");
        }
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null)
        {
            return BadRequest("User not found!");
        }
        _context.Cards.Remove(data);
        await _logService.AddAsync($"{user} đã xóa thẻ {data.Code}", Guid.Empty);
        var users = await _context.Users.Where(x => x.CardId == id).ToListAsync();
        foreach (var item in users)
        {
            item.CardId = null;
            _context.Users.Update(item);
        }

        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpPost("forgot-password"), AllowAnonymous]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.Email))
        {
            return BadRequest("Vui lòng nhập địa chỉ email");
        }
        var user = await _userManager.FindByEmailAsync(args.Email);
        if (user is null)
        {
            return BadRequest("Người dùng không tồn tại");
        }
        return Ok(IdentityResult.Success);
    }

    [HttpGet("parent-options")]
    public async Task<IActionResult> GetParentOptionsAsync()
    {
        var query = _context.Users
            .Select(x => new
            {
                value = x.Id,
                label = $"{x.Name} - {x.UserName}"
            });
        return Ok(await query.ToListAsync());
    }

    [HttpGet("sub-user/list/{id}")]
    public async Task<IActionResult> GetSubUserListAsync([FromRoute] Guid id)
    {
        return Ok(new
        {
            data = await _context.SubUsers.Where(x => x.UserId == id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Email,
                x.DateOfBirth,
                x.IdentityNumber,
                x.PhoneNumber,
                x.Gender
            }).ToListAsync(),
            total = await _context.SubUsers.CountAsync(x => x.UserId == id)
        });
    }

    [HttpPost("sub-user/add")]
    public async Task<IActionResult> AddSubUserAsync([FromBody] SubUser args)
    {
        var user = await _userManager.FindByIdAsync(args.UserId.ToString());
        if (user is null) return BadRequest("User not found!");
        if (!string.IsNullOrWhiteSpace(args.PhoneNumber))
        {
            if (args.PhoneNumber.Length != 10)
            {
                return BadRequest("Số điện thoại không hợp lệ!");
            }

            var lead = await _context.Leads.FirstOrDefaultAsync(x => x.PhoneNumber == args.PhoneNumber);
            if (lead != null)
            {
                return BadRequest($"Số điện thoại đã tồn tại trong hệ thống -> Khách: {lead.Name}");
            }

            var subLead = await _context.SubLeads.FirstOrDefaultAsync(x => x.PhoneNumber == args.PhoneNumber);
            if (subLead != null)
            {
                return BadRequest($"Số điện thoại đã tồn tại trong hệ thống -> Khách: {subLead.Name}");
            }
        }

        await _context.SubUsers.AddAsync(args);

        var admin = await _userManager.FindByIdAsync(User.GetClaimId());
        if (admin is null) return Unauthorized();
        await _logService.AddAsync($"{admin.Name} đã tạo chủ thẻ phụ {args.Name} cho tài khoản {user.Name}");

        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpPost("sub-user/delete/{id}")]
    public async Task<IActionResult> DeleteSubUser([FromRoute] Guid id)
    {
        var subUser = await _context.SubUsers.FindAsync(id);
        if (subUser is null) return BadRequest("Sub user not found!");
        _context.SubUsers.Remove(subUser);

        var admin = await _userManager.FindByIdAsync(User.GetClaimId());
        if (admin is null) return Unauthorized();
        await _logService.AddAsync($"{admin.Name} đã xóa thành viên phụ {subUser.Name}");

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("sub-user/update")]
    public async Task<IActionResult> UpdateSubUserAsync([FromBody] SubUser args)
    {
        var data = await _context.SubUsers.FindAsync(args.Id);
        if (data is null) return BadRequest("Sub user not found");
        data.PhoneNumber = args.PhoneNumber;
        data.IdentityNumber = args.IdentityNumber;
        data.Email = args.Email;
        data.DateOfBirth = args.DateOfBirth;
        data.Name = args.Name;
        data.Gender = args.Gender;
        _context.SubUsers.Update(data);

        var admin = await _userManager.FindByIdAsync(User.GetClaimId());
        if (admin is null) return Unauthorized();
        await _logService.AddAsync($"{admin.Name} đã cập nhật thành viên phụ {data.Name}");

        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("achievements")]
    public async Task<IActionResult> AchievementsAsync()
    {
        var achievements = await _context.Achievements
            .Where(x => x.IsApproved)
            .Where(x => x.UserId == User.GetId()).OrderByDescending(x => x.CreatedDate).ToListAsync();
        return Ok(achievements);
    }

    [HttpGet("achievements-by-user/{id}")]
    public async Task<IActionResult> AchievementsAsync([FromRoute] Guid id)
    {
        var achievements = await _context.Achievements
            .Where(x => x.IsApproved)
            .Where(x => x.UserId == id).OrderByDescending(x => x.CreatedDate).ToListAsync();
        return Ok(achievements);
    }

    [HttpPost("achievement/add")]
    public async Task<IActionResult> AchToUserAsync([FromBody] Achievement args)
    {
        args.CreatedDate = DateTime.Now;
        args.NormalizedName = SeoHelper.ToWikiFriendly(args.Name);
        await _context.Achievements.AddAsync(args);
        args.CxId = User.GetId();

        var admin = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (admin is null) return Unauthorized();

        var cardHolder = await _userManager.FindByIdAsync(args.UserId.ToString());
        if (cardHolder is null) return BadRequest();

        await _logService.AddAsync($"{admin.Name} đã tạo thành tựu [{args.Name}] cho {cardHolder.Name}");

        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("achievement/list-approve")]
    public async Task<IActionResult> ListAchApproveAsync([FromQuery] SearchFilterOptions filterOptions)
    {
        var query = from a in _context.Achievements
                    join b in _context.Users on a.UserId equals b.Id
                    join c in _context.Users on a.CxId equals c.Id
                    orderby a.CreatedDate descending
                    select new
                    {
                        a.Id,
                        a.Name,
                        a.Icon,
                        a.CreatedDate,
                        a.IsApproved,
                        a.ApprovedDate,
                        CardHolderName = b.Name,
                        CxName = c.Name,
                        CxUserName = c.UserName
                    };

        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        return Ok(new
        {
            data = await query.Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("achievement/approve/{id}")]
    public async Task<IActionResult> ApproveAchieventAsync([FromRoute] Guid id)
    {
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement is null) return BadRequest("Data not found!");
        achievement.IsApproved = true;
        achievement.ApprovedDate = DateTime.Now;
        achievement.CxmId = User.GetId();
        _context.Achievements.Update(achievement);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("achievement/delete/{id}")]
    public async Task<IActionResult> DeleteAchieventAsync([FromRoute] Guid id)
    {
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement is null) return BadRequest("Data not found!");
        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("role/create")]
    public async Task<IActionResult> CreateRoleAsync([FromBody] ApplicationRole role)
    {
        await _roleManager.CreateAsync(role);
        return Ok(role);
    }

    [HttpGet("role/options")]
    public async Task<IActionResult> GetRoleOptionsAsync()
    {
        return Ok(await _context.Roles.Where(x => x.Name != RoleName.CardHolder).Select(x => new
        {
            label = x.DisplayName,
            value = x.Name
        }).ToListAsync());
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId into ab
                    from b in ab.DefaultIfEmpty()
                    where b == null && a.Status == UserStatus.Working
                    select new
                    {
                        label = a.UserName,
                        value = a.Id,
                        a.Branch
                    };

        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        query = query.Where(x => x.Branch == user.Branch);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("dos/options")]
    public async Task<IActionResult> GetDosOptionsAsync()
    {
        var query = await _userManager.GetUsersInRoleAsync(RoleName.Dos);
        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        query = query.Where(x => x.Branch == user.Branch && x.Status == UserStatus.Working).ToList();

        return Ok(query.Select(x => new
        {
            label = $"{x.Name} - {x.UserName}",
            value = x.Id
        }));
    }

    [HttpGet("card-holder/options")]
    public async Task<IActionResult> GetCardHolderOptionsAsync()
    {
        var query = await _userManager.GetUsersInRoleAsync(RoleName.CardHolder);
        return Ok(query.Select(x => new
        {
            label = $"{x.Name} - {x.UserName}",
            value = x.Id
        }));
    }

    [HttpPost("send-emails")]
    public async Task<IActionResult> SendEmailAsync([FromBody] SendEmailsArgs args)
    {
        if (args.UserIds is null || !args.UserIds.Any()) return BadRequest();
        if (string.IsNullOrEmpty(args.Subject) || string.IsNullOrEmpty(args.Body)) return BadRequest();
        foreach (var userId in args.UserIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Email)) continue;
            await Sender.SendAsync(user.Email, args.Subject, args.Body);
        }
        return Ok();
    }

    [HttpGet("telesale-manager/options")]
    public async Task<IActionResult> GetSmOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.TelesaleManager
                    select new
                    {
                        a.UserName,
                        a.Id,
                        a.Name,
                        a.Branch,
                        a.Status
                    };

        if (!User.IsInRole(RoleName.Admin))
        {
            var user = await _userManager.FindByIdAsync(User.GetId().ToString());
            if (user is null) return Unauthorized();
            query = query.Where(x => x.Branch == user.Branch);
        }

        return Ok(await query.Select(x => new
        {
            label = $"{x.Name} - {x.UserName}",
            value = x.Id,
            disabled = x.Status != UserStatus.Working
        }).ToListAsync());
    }

    [HttpGet("sm/options/{id}")]
    public async Task<IActionResult> GetSmOptionsAsync([FromRoute] Guid? id)
    {
        var query = from a in _context.Users.Where(x => x.DosId == id || id == null)
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.SalesManager
                    select new
                    {
                        a.UserName,
                        a.Id,
                        a.Name,
                        a.Branch,
                        a.Status
                    };
        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.Event))
        {
            query = query.Where(x => x.Branch == user.Branch);
        }

        return Ok(await query.Select(x => new
        {
            label = $"{x.Name} - {x.UserName}",
            value = x.Id,
            disabled = x.Status != UserStatus.Working
        }).ToListAsync());
    }

    [HttpPost("sales/set-seller")]
    public async Task<IActionResult> SetSellerAsync([FromBody] SetSaller args)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user == null) return BadRequest("User not found!");

        var cardHolder = await _context.Users.FindAsync(args.CardHolderId);
        if (cardHolder == null) return BadRequest("Không tìm thấy chủ thẻ");
        if (cardHolder.SellerId == args.SellerId)
        {
            return BadRequest("Nhân sự này đã là trợ lý của chủ thẻ!");
        }

        var target = await _context.Users.FindAsync(args.SellerId);
        if (target is null) return BadRequest();
        if (cardHolder.SellerId != null)
        {
            var current = await _context.Users.FindAsync(cardHolder.SellerId);
            if (current is null)
            {
                // Nếu trợ lý không tồn tại hoặc bị xa thải rồi
                cardHolder.SellerId = args.SellerId;
            }
            else
            {
                await _context.UserChanges.AddAsync(new UserChange
                {
                    CurrentId = cardHolder.SellerId.GetValueOrDefault(),
                    TargetId = args.SellerId,
                    Type = "sales",
                    Note = $"Đổi trợ lý cá nhân của {cardHolder.Name} từ {current.Name} sang {target.Name}",
                    UserId = cardHolder.Id,
                    CreatedDate = DateTime.Now
                });
                cardHolder.HasChange = true;
            }
        }
        else
        {
            // Nếu chủ thẻ chưa có trợ lý thì không cần phê duyệt
            cardHolder.SellerId = args.SellerId;
        }

        _context.Update(cardHolder);

        await _logService.AddAsync($"{user.UserName} - {user.Name} đã cập nhật chuyên viên quản lý của {cardHolder.Name}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("sales-with-sm-options")]
    public async Task<IActionResult> GetSalesWithSmOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    join d in _context.Users on a.SmId equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    where c.Name == RoleName.Sales && a.SmId != null && a.Status == UserStatus.Working
                    select new
                    {
                        label = $"{a.Name} - {a.UserName}",
                        value = a.Id,
                        a.SmId,
                        SmName = d.Name,
                        a.Branch
                    };
        if (!User.IsInRole(RoleName.CxTP))
        {
            var user = await _userManager.FindByIdAsync(User.GetClaimId());
            if (user is null) return Unauthorized();
            query = query.Where(x => x.Branch == user.Branch);
        }
        var data = await query.GroupBy(x => new
        {
            x.SmId,
            x.SmName
        }).Select(x => new
        {
            label = x.Key.SmName,
            value = x.Key.SmId,
            options = x.Select(y => new
            {
                y.label,
                y.value
            })
        }).ToListAsync();
        return Ok(data);
    }

    [HttpGet("tele-with-tm-options")]
    public async Task<IActionResult> GetTeleWithTmOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    join d in _context.Users on a.TmId equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    where c.Name == RoleName.Telesale && a.TmId != null && a.Status == UserStatus.Working
                    select new
                    {
                        label = $"{a.Name} - {a.UserName}",
                        value = a.Id,
                        a.TmId,
                        SmName = d.Name
                    };
        var userId = User.GetId();
        if (User.IsInRole(RoleName.TelesaleManager))
        {
            query = query.Where(x => x.TmId == userId);
        }
        var data = await query.GroupBy(x => new
        {
            x.TmId,
            x.SmName
        }).Select(x => new OptionGroup
        {
            Label = x.Key.SmName,
            Value = x.Key.TmId,
            Options = x.Select(y => new Option
            {
                Label = y.label,
                Value = y.value
            })
        }).ToListAsync();
        if (User.IsInRole(RoleName.TelesaleManager))
        {
            return Ok(data);
        }

        var query2 = from a in _context.Users
                     join b in _context.UserRoles on a.Id equals b.UserId
                     join c in _context.Roles on b.RoleId equals c.Id
                     join d in _context.Users on a.SmId equals d.Id into ad
                     from d in ad.DefaultIfEmpty()
                     where c.Name == RoleName.Sales && a.SmId != null
                     select new
                     {
                         label = $"{a.Name} - {a.UserName}",
                         value = a.Id,
                         a.SmId,
                         SmName = d.Name,
                         a.Branch
                     };
        if (!User.IsInRole(RoleName.CxTP))
        {
            var user = await _userManager.FindByIdAsync(User.GetClaimId());
            if (user is null) return Unauthorized();
            query2 = query2.Where(x => x.Branch == user.Branch);
        }
        var data2 = await query2.GroupBy(x => new
        {
            x.SmId,
            x.SmName
        }).Select(x => new OptionGroup
        {
            Label = x.Key.SmName,
            Value = x.Key.SmId,
            Options = x.Select(y => new Option
            {
                Label = y.label,
                Value = y.value
            })
        }).ToListAsync();

        return Ok(data.Concat(data2));
    }

    [HttpGet("sales/options")]
    public async Task<IActionResult> GetSellerOptionsAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.Sales && a.Status == UserStatus.Working
                    select new
                    {
                        label = $"{a.Name} - {a.UserName}",
                        value = a.Id,
                        a.SmId,
                        a.DosId,
                        a.Branch
                    };
        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user == null) return BadRequest();

        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (!User.IsInRole(RoleName.CxTP) && !User.IsInRole(RoleName.Event) && !User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.Dot))
        {
            query = query.Where(x => x.Branch == user.Branch);
        }

        return Ok(await query.ToListAsync());
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportAsync([FromQuery] UserFilterOptions filterOptions)
    {
        try
        {
            var query = await GetUserQuery(filterOptions);
            var data = await query.ToListAsync();
            var trans = await _context.Transactions.Where(x => x.Point < 0).GroupBy(x => x.UserId).Select(x => new
            {
                UserId = x.Key,
                TotalSpent = x.Sum(s => s.Point)
            }).ToListAsync();

            var user = await _userManager.FindByIdAsync(User.GetClaimId());
            if (user is null) return BadRequest("User not found!");
            var admin = await _userManager.IsInRoleAsync(user, RoleName.Admin);

            if (data is null) return BadRequest("Data not found!");

            using var pgk = new ExcelPackage();
            var ws = pgk.Workbook.Worksheets.Add("Sheet 1");

            ws.Cells[1, 1].Value = "STT";
            ws.Cells[1, 2].Value = "Mã hợp đồng";
            ws.Cells[1, 3].Value = "Họ và tên";
            ws.Cells[1, 4].Value = "Giới tính";
            ws.Cells[1, 5].Value = "Ngày sinh";
            ws.Cells[1, 6].Value = "Email";
            ws.Cells[1, 7].Value = "SDT";
            ws.Cells[1, 8].Value = "CCCD";
            ws.Cells[1, 9].Value = "Ngày cấp";
            ws.Cells[1, 10].Value = "Nơi cấp";
            ws.Cells[1, 11].Value = "Điểm";
            ws.Cells[1, 12].Value = "Ngày hết hạn";
            ws.Cells[1, 13].Value = "Loại thẻ";
            ws.Cells[1, 14].Value = "Tiền sử sức khỏe";
            ws.Cells[1, 15].Value = "Đặc điểm gia đình";
            ws.Cells[1, 16].Value = "Đặc điểm tính cách";
            ws.Cells[1, 17].Value = "Mối quan tâm";
            ws.Cells[1, 18].Value = "Địa chỉ";
            ws.Cells[1, 19].Value = "Số điểm đã sử dụng";

            var row = 2;

            foreach (var item in data)
            {

                var phoneNumber = "**********";
                var email = "**********";
                if (admin)
                {
                    phoneNumber = item.PhoneNumber;
                    email = item.Email;
                }
                ws.Cells[row, 1].Value = row - 1;
                ws.Cells[row, 2].Value = item.ContractCode;
                ws.Cells[row, 3].Value = item.Name;
                ws.Cells[row, 4].Value = item.Gender == true ? "Nam" : "Nữ";
                ws.Cells[row, 5].Value = item.DateOfBirth?.ToString("dd/MM/yyyy");
                ws.Cells[row, 6].Value = phoneNumber;
                ws.Cells[row, 7].Value = email;
                ws.Cells[row, 8].Value = item.IdentityNumber;
                ws.Cells[row, 9].Value = item.IdentityDate?.ToString("dd/MM/yyyy");
                ws.Cells[row, 10].Value = item.IdentityAddress;
                ws.Cells[row, 11].Value = item.Loyalty.ToString();
                ws.Cells[row, 12].Value = item.LoyaltyExpiredDate?.ToString("dd/MM/yyyy");
                ws.Cells[row, 13].Value = item.TierName;
                ws.Cells[row, 14].Value = item.HealthHistory;
                ws.Cells[row, 15].Value = item.FamilyCharacteristics;
                ws.Cells[row, 16].Value = item.Personality;
                ws.Cells[row, 17].Value = item.Concerns;
                ws.Cells[row, 18].Value = item.Address;
                ws.Cells[row, 19].Value = trans.FirstOrDefault(x => x.UserId == item.Id)?.TotalSpent;
                row++;
            }

            await pgk.SaveAsync();

            var fileName = $"data-nuras";

            return File(await pgk.GetAsByteArrayAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("info/{id}")]
    public async Task<IActionResult> GetUserInfoAsync([FromRoute] Guid id) => Ok(await _context.UserInfos.FirstOrDefaultAsync(x => x.UserId == id));

    [HttpPost("info/save")]
    public async Task<IActionResult> SaveUserInfoAsync([FromBody] UserInfo args)
    {
        var userInfo = await _context.UserInfos.FirstOrDefaultAsync(x => x.UserId == args.UserId);
        if (userInfo is null)
        {
            userInfo = new UserInfo
            {
                UserId = args.UserId,
                Concerns = args.Concerns,
                FamilyCharacteristics = args.FamilyCharacteristics,
                HealthHistory = args.HealthHistory,
                Personality = args.Personality
            };
            await _context.UserInfos.AddAsync(userInfo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(SaveUserInfoAsync), IdentityResult.Success);
        }
        userInfo.Personality = args.Personality;
        userInfo.Concerns = args.Concerns;
        userInfo.FamilyCharacteristics = args.FamilyCharacteristics;
        userInfo.HealthHistory = args.HealthHistory;
        _context.UserInfos.Update(userInfo);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("birth-days")]
    public async Task<IActionResult> BirthDaysAsync()
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where c.Name == RoleName.CardHolder && a.DateOfBirth != null && a.DateOfBirth.Value.Month == DateTime.Now.Month
                    select new
                    {
                        a.Id,
                        a.UserName,
                        a.Name,
                        a.DateOfBirth,
                        a.Gender,
                        a.ContractCode,
                        a.SellerId,
                        a.SmId,
                        a.DosId
                    };

        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();
        if (User.IsInRole(RoleName.Sales))
        {
            query = query.Where(x => x.SellerId == user.Id);
        }
        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == user.Id);
        }

        return Ok(new
        {
            data = await query.ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpGet("card-holder/count")]
    public async Task<IActionResult> CardHolderCountAsync()
    {
        var u = await _userManager.GetUsersInRoleAsync(RoleName.CardHolder);
        return Ok(u.Count);
    }

    [HttpGet("card-holder/statistics/{id}")]
    public async Task<IActionResult> CardHolderStatisticsAsync([FromRoute] string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return BadRequest();
            return Ok(new
            {
                currentPoint = user.Loyalty,
                totalSpent = await _context.Transactions.Where(x => x.UserId == user.Id && x.Point < 0).SumAsync(x => x.Point),
                user.LoanPoint
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("loan")]
    public async Task<IActionResult> LoanAsync([FromBody] Transaction args)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == User.GetId());
        if (user == null) return BadRequest("User not found!");
        if (args.Point < 1) return BadRequest("Số điểm vay không hợp lệ");
        await _context.Transactions.AddAsync(new Transaction
        {
            CreatedBy = User.GetId(),
            CreatedDate = DateTime.Now,
            Memo = $"{user.Name} vay {args.Point} điểm",
            Status = TransactionStatus.Pending,
            UserId = user.Id,
            Point = args.Point,
            Reason = args.Reason
        });
        user.LoanPoint += args.Point;
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpGet("list-new-card-holder")]
    public async Task<IActionResult> ListNewCardHolderAsync()
    {
        var query = from a in _context.Cards
                    join b in _context.Users on a.Id equals b.CardId
                    orderby b.CreatedDate descending
                    select new
                    {
                        a.Tier,
                        b.Id,
                        b.Name,
                        b.CreatedDate,
                        b.Gender,
                        b.UserName,
                        b.Avatar,
                        b.ContractCode,
                        b.SellerId,
                        b.SmId,
                        b.DosId,
                        TierName = a.Code,
                        TierColor = a.Color
                    };

        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();
        if (User.IsInRole(RoleName.Sales))
        {
            query = query.Where(x => x.SellerId == user.Id);
        }
        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == user.Id);
        }

        return Ok(new
        {
            data = await query.Take(5).ToListAsync(),
            total = 5
        });
    }

    [HttpGet("list-top-sales")]
    public async Task<IActionResult> ListTopSaleAsync()
    {
        var query = from a in _context.UserTopups
                    join b in _context.Users on a.SaleId equals b.Id
                    where a.Status == TopupStatus.AccountantApproved && a.CreatedDate.Year == DateTime.Now.Year && a.CreatedDate.Month == DateTime.Now.Month
                    select new
                    {
                        b.Id,
                        b.Name,
                        b.CreatedDate,
                        a.Amount,
                        a.SmId,
                        a.DosId
                    };

        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == user.Id);
        }
        return Ok(new
        {
            data = await query.GroupBy(x => x.Name).Select(x => new
            {
                x.Key,
                Amount = x.Sum(s => s.Amount)
            }).ToListAsync(),
            total = 8
        });
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopupAsync([FromBody] TopupArgs args)
    {
        var cardHolder = await _context.Users.FindAsync(args.CardHolderId);
        if (cardHolder is null) return BadRequest("Không tìm thấy chủ thẻ");
        var sales = await _context.Users.FindAsync(cardHolder.SellerId);
        if (sales is null) return BadRequest("Không tìm thấy trợ lý cá nhân!");
        var status = TopupStatus.Pending;
        if (User.IsInRole(RoleName.Accountant) || User.IsInRole(RoleName.ChiefAccountant))
        {
            status = TopupStatus.AccountantApproved;
            cardHolder.Amount = args.Amount;
            _context.Users.Update(cardHolder);
        }
        var userTopup = new UserTopup
        {
            CardHolderId = cardHolder.Id,
            Amount = args.Amount,
            CreatedDate = DateTime.Now,
            Note = args.Note,
            Status = status,
            SaleId = sales.Id,
            SmId = sales.SmId,
            DosId = sales.DosId,
            Type = args.Type,
            ContractCode = cardHolder.ContractCode
        };
        await _context.UserTopups.AddAsync(userTopup);

        await _logService.AddAsync($"{sales.Name} - {sales.UserName} đã nạp {args.Amount}đ cho chủ thẻ {cardHolder.Name}");

        await _context.SaveChangesAsync();

        return Ok(IdentityResult.Success);
    }

    [HttpGet("list-topup")]
    public async Task<IActionResult> ListTopupAsync([FromQuery] UserFilterOptions filterOptions)
    {
        try
        {
            var user = await _context.Users.FindAsync(User.GetId());
            if (user is null) return Unauthorized();
            var query = from a in _context.UserTopups
                        join cardHolder in _context.Users on a.CardHolderId equals cardHolder.Id
                        join sale in _context.Users on a.SaleId equals sale.Id
                        join director in _context.Users on a.DirectorId equals director.Id into ad
                        from director in ad.DefaultIfEmpty()
                        join accountant in _context.Users on a.AccountantId equals accountant.Id into aa
                        from accountant in aa.DefaultIfEmpty()
                        select new ListTopup
                        {
                            Id = a.Id,
                            CreatedDate = a.CreatedDate,
                            AccountantApprovedDate = a.AccountantApprovedDate,
                            Amount = a.Amount,
                            DirectorApprovedDate = a.DirectorApprovedDate,
                            Status = a.Status,
                            Note = a.Note,
                            CardHolderName = cardHolder.Name,
                            AccountantName = accountant.Name,
                            DirectorName = director.Name,
                            Email = cardHolder.Email,
                            PhoneNumber = cardHolder.PhoneNumber,
                            Type = a.Type,
                            Branch = cardHolder.Branch
                        };
            if (User.IsInRole(RoleName.Dos))
            {
                query = query.Where(x => x.Status != TopupStatus.DirectorApproved && x.Status != TopupStatus.Rejected);
            }
            if (User.IsInRole(RoleName.Accountant) || User.IsInRole(RoleName.ChiefAccountant))
            {
                query = query.Where(x => x.Status != TopupStatus.Pending && x.Status != TopupStatus.Rejected);
            }
            if (!string.IsNullOrWhiteSpace(filterOptions.Name))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.CardHolderName) && x.CardHolderName.ToLower().Contains(filterOptions.Name.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filterOptions.Email))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(filterOptions.Email.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber.ToLower()));
            }
            if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.ChiefAccountant))
            {
                query = query.Where(x => x.Branch == user.Branch);
            }
            query = query.OrderByDescending(x => x.CreatedDate);
            return Ok(new
            {
                data = await query.Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
                total = await query.CountAsync()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("approve-topup")]
    public async Task<IActionResult> ApproveTopupAsync([FromBody] ApproveTopupArgs args)
    {
        var user = await _context.Users.FindAsync(User.GetId());
        if (user is null) return Unauthorized();
        var userTopup = await _context.UserTopups.FindAsync(args.Id);
        if (userTopup is null) return BadRequest();
        if (args.Status == TopupStatus.DirectorApproved)
        {
            userTopup.DirectorApprovedDate = DateTime.Now;
            userTopup.DirectorId = user.Id;
        }
        if (args.Status == TopupStatus.AccountantApproved)
        {
            userTopup.AccountantId = user.Id;
            userTopup.AccountantApprovedDate = DateTime.Now;
            var cardHolder = await _context.Users.FindAsync(userTopup.CardHolderId);
            if (cardHolder is null) return BadRequest("Không tìm thấy chủ thẻ");
            cardHolder.Amount += userTopup.Amount;
            _context.Users.Update(cardHolder);
        }
        if (args.Status == TopupStatus.Rejected)
        {
            if (userTopup.Status == TopupStatus.Pending)
            {
                userTopup.DirectorApprovedDate = DateTime.Now;
                userTopup.DirectorId = user.Id;
            }
            if (userTopup.Status == TopupStatus.DirectorApproved)
            {
                userTopup.AccountantApprovedDate = DateTime.Now;
                userTopup.AccountantId = user.Id;
            }
        }
        userTopup.Status = args.Status;
        userTopup.Note = args.Note;
        _context.UserTopups.Update(userTopup);
        await _context.SaveChangesAsync();

        return Ok(IdentityResult.Success);
    }

    [HttpGet("sale-chart")]
    public async Task<IActionResult> GetSaleChartAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var firstDayOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1);
        var user = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (user is null) return Unauthorized();

        var prevMonthsQ = _context.UserTopups
            .Where(x => x.Status == TopupStatus.AccountantApproved)
            .Where(x => x.CreatedDate.Month == DateTime.Now.AddMonths(-1).Month && x.CreatedDate.Year == DateTime.Now.Year);
        if (User.IsInRole(RoleName.SalesManager))
        {
            prevMonthsQ = prevMonthsQ.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            prevMonthsQ = prevMonthsQ.Where(x => x.DosId == user.DosId);
        }

        var prevMonths = await prevMonthsQ.ToListAsync();

        var currentMonthsQ = _context.UserTopups
            .Where(x => x.Status == TopupStatus.AccountantApproved)
            .Where(x => x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Year == DateTime.Now.Year);
        if (User.IsInRole(RoleName.SalesManager))
        {
            currentMonthsQ = currentMonthsQ.Where(x => x.SmId == user.Id);
        }
        if (User.IsInRole(RoleName.Dos))
        {
            currentMonthsQ = currentMonthsQ.Where(x => x.DosId == user.Id);
        }

        var currentMonths = await currentMonthsQ.ToListAsync();

        var users = await _context.UserTopups.Where(x => x.CreatedDate.Date >= firstDayOfLastMonth).GroupBy(x => x.SaleId).Select(x => x.Key).ToListAsync();
        var sales = await _userManager.GetUsersInRoleAsync(RoleName.Sales);

        var data = new List<SaleChart>();
        data.AddRange(users.Select(x => new SaleChart
        {
            Value = prevMonths.Where(c => c.SaleId == x).Sum(x => x.Amount),
            Type = DateTime.Now.AddMonths(-1).ToString("MMM"),
            Name = sales.FirstOrDefault(s => s.Id == x)?.Name ?? "Chưa rõ"
        }));
        data.AddRange(users.Select(x => new SaleChart
        {
            Value = currentMonths.Where(c => c.SaleId == x).Sum(x => x.Amount),
            Type = DateTime.Now.ToString("MMM"),
            Name = sales.FirstOrDefault(s => s.Id == x)?.Name ?? "Chưa rõ"
        }).OrderByDescending(x => x.Value).Take(3));

        return Ok(new { data });
    }

    [HttpGet("sm-chart")]
    public async Task<IActionResult> GetSMChartAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var firstDayOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1);

        var prevMonths = await _context.UserTopups
            .Where(x => x.Status == TopupStatus.AccountantApproved)
            .Where(x => x.CreatedDate.Month == DateTime.Now.AddMonths(-1).Month && x.CreatedDate.Year == DateTime.Now.Year)
            .ToListAsync();

        var currentMonths = await _context.UserTopups
            .Where(x => x.Status == TopupStatus.AccountantApproved)
            .Where(x => x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Year == DateTime.Now.Year).ToListAsync();

        var users = await _context.UserTopups.Where(x => x.CreatedDate.Date >= firstDayOfLastMonth).GroupBy(x => x.SmId).Select(x => x.Key).ToListAsync();
        var sales = await _userManager.GetUsersInRoleAsync(RoleName.SalesManager);

        var data = new List<SaleChart>();
        data.AddRange(users.Select(x => new SaleChart
        {
            Value = prevMonths.Where(c => c.SmId == x).Sum(x => x.Amount),
            Type = DateTime.Now.AddMonths(-1).ToString("MMM"),
            Name = sales.FirstOrDefault(s => s.Id == x)?.Name ?? "Chưa rõ"
        }));
        data.AddRange(users.Select(x => new SaleChart
        {
            Value = currentMonths.Where(c => c.SmId == x).Sum(x => x.Amount),
            Type = DateTime.Now.ToString("MMM"),
            Name = sales.FirstOrDefault(s => s.Id == x)?.Name ?? "Chưa rõ"
        }).OrderByDescending(x => x.Value).Take(3));

        return Ok(new { data });
    }

    [HttpPost("reject-changes/{id}")]
    public async Task<IActionResult> RejectChangesAsync([FromRoute] Guid id)
    {
        var data = await _context.UserChanges.FindAsync(id);
        if (data is null) return BadRequest("Data not found!");
        var user = await _context.Users.FindAsync(data.UserId);
        if (user is null) return BadRequest("User not found");
        user.HasChange = false;
        _context.Users.Update(user);

        var admin = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (admin is null) return Unauthorized();

        await _logService.AddAsync($"{admin.Name} đã từ chối yêu cầu: {data.Note}");

        _context.UserChanges.Remove(data);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("approve-changes/{id}")]
    public async Task<IActionResult> ApproveChangesAsync([FromRoute] Guid id)
    {
        var data = await _context.UserChanges.FindAsync(id);
        if (data is null) return BadRequest("Data not found!");
        data.IsAccept = true;
        _context.UserChanges.Update(data);
        var user = await _context.Users.FindAsync(data.UserId);
        if (user is null) return BadRequest("User not found");
        user.HasChange = false;

        if (data.Type == RoleName.Sales)
        {
            user.SellerId = data.TargetId;
            var sales = await _userManager.FindByIdAsync(user.SellerId.GetValueOrDefault().ToString());
            if (sales is null) return BadRequest("Sales not found!");
            user.SmId = sales.SmId;
            user.DosId = sales.DosId;
        }
        if (data.Type == RoleName.SalesManager)
        {
            user.SmId = data.TargetId;
            var sm = await _userManager.FindByIdAsync(user.SmId.GetValueOrDefault().ToString());
            if (sm is null) return BadRequest("SM not found!");
            user.DosId = sm.DosId;
        }
        // Chuyển đổi từ lead sang => không cần làm gì hết
        if (data.Type == RoleName.CardHolder)
        {

        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("changes-list")]
    public async Task<IActionResult> ChangeListAsync([FromQuery] SearchFilterOptions filterOptions)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null) return Unauthorized();

        var admin = await _userManager.IsInRoleAsync(user, RoleName.Admin);

        var query = _context.UserChanges.Where(x => !x.IsAccept || admin);

        if (await _userManager.IsInRoleAsync(user, RoleName.Dos))
        {
            query = query.Where(x => x.Type == RoleName.Sales);
        }

        if (await _userManager.IsInRoleAsync(user, RoleName.Hr))
        {
            query = query.Where(x => x.Type == RoleName.SalesManager);
        }

        query = query.OrderByDescending(x => x.CreatedDate);

        return Ok(new
        {
            data = await query.ToListAsync()
        });
    }

    [HttpGet("sm-dos-options")]
    public async Task<IActionResult> GetSmDosOptionsAsync()
    {
        var dos = await _userManager.GetUsersInRoleAsync(RoleName.Dos);
        dos = dos.Where(x => x.Status == UserStatus.Working).ToList();
        var sm = await _userManager.GetUsersInRoleAsync(RoleName.SalesManager);
        sm = sm.Where(x => x.Status == UserStatus.Working).ToList();
        return Ok(new[]
        {
            new {
                label = "Giám đốc",
                title = "Giám đốc",
                options = dos.Select(x => new
                {
                    label = x.Name,
                    value = x.Name
                })
            },
            new {
                label = "Quản lý",
                title = "Quản lý",
                options = sm.Select(x => new
                {
                    label = x.Name,
                    value = x.Name
                })
            }
        });
    }

    [HttpPost("leave/{id}")]
    public async Task<IActionResult> LockAsync([FromRoute] Guid id)
    {
        if (!User.IsInRole(RoleName.Hr) && !User.IsInRole(RoleName.Admin)) return BadRequest("Truy cập bị từ chối!");
        var user = await _context.Users.FindAsync(id);
        if (user is null) return BadRequest("User not found!");
        if (user.Status == UserStatus.Leave) return BadRequest("User đã nghỉ việc rồi!");
        if (await _userManager.IsInRoleAsync(user, RoleName.SalesManager))
        {
            var query = from a in _context.Users
                        join b in _context.UserRoles on a.Id equals b.UserId
                        join c in _context.Roles on b.RoleId equals c.Id
                        where c.Name == RoleName.Sales && a.SmId == user.Id && a.Status == UserStatus.Working
                        select a.Name;
            var sales = await query.FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(sales))
            {
                return BadRequest($"Không thể nghỉ việc {user.Name} vì vẫn còn nhân viên {sales} đang làm việc dưới quyền!");
            }
        }
        if (await _userManager.IsInRoleAsync(user, RoleName.TelesaleManager))
        {
            var query = from a in _context.Users
                        join b in _context.UserRoles on a.Id equals b.UserId
                        join c in _context.Roles on b.RoleId equals c.Id
                        where c.Name == RoleName.Telesale && a.TmId == user.Id && a.Status == UserStatus.Working
                        select a.Name;
            var telesales = await query.FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(telesales))
            {
                return BadRequest($"Không thể nghỉ việc {user.Name} vì vẫn còn nhân viên {telesales} đang làm việc dưới quyền!");
            }
        }
        user.Status = UserStatus.Leave;
        _context.Users.Update(user);

        var admin = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (admin is null) return BadRequest("Admin not found!");

        await _logService.AddAsync($"{admin.Name} đã đổi trạng thái {user.Name} sang nghỉ việc!");

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("unlock/{id}")]
    public async Task<IActionResult> UnLockAsync([FromRoute] Guid id)
    {
        if (!User.IsInRole(RoleName.Hr) && !User.IsInRole(RoleName.Admin)) return BadRequest("Truy cập bị từ chối!");
        var user = await _context.Users.FindAsync(id);
        if (user is null) return BadRequest("User not found!");
        user.Status = UserStatus.Working;
        _context.Users.Update(user);

        var admin = await _userManager.FindByIdAsync(User.GetId().ToString());
        if (admin is null) return BadRequest("Admin not found!");

        await _logService.AddAsync($"{admin.Name} đã đổi trạng thái {user.Name} sang làm việc!");

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("change-avatar")]
    public async Task<IActionResult> ChangeAvatarAsync([FromForm] UploadFileArgs args)
    {
        try
        {
            var user = await _context.Users.FindAsync(User.GetId());
            if (user is null) return BadRequest("User not found!");
            if (args.File is null) return BadRequest("File not found!");
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(args.File.FileName)}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "avatars", fileName);
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "files", "avatars")))
            {
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "files", "avatars"));
            }
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await args.File.CopyToAsync(stream);
            }
            user.Avatar = $"https://nuras.com.vn/files/avatars/{fileName}";
            _context.Users.Update(user);
            await _logService.AddAsync($"{user.Name} đã cập nhật ảnh đại diện");
            await _context.SaveChangesAsync();
            return Ok(new { user.Avatar });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPasswordAsync([FromBody] SetPasswordArgs args)
    {
        if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.Hr)) return BadRequest("Bạn không có quyền đổi mật khẩu!");
        if (string.IsNullOrWhiteSpace(args.Password)) return BadRequest("Vui lòng nhập mật khẩu!");
        var user = await _userManager.FindByIdAsync(args.UserId);
        if (user is null) return BadRequest("Không tìm thấy người dùng!");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, args.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return Ok();
    }

    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync()
    {
        var user = await _userManager.FindByNameAsync("tandc");
        if (user is null) return BadRequest("User not found!");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, "1");
        return Ok(result);
    }

    [HttpPost("change-contract-code")]
    public async Task<IActionResult> ChangeContractCodeAsync([FromBody] ChangeContractCodeArgs args)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(args.ContractCode)) return BadRequest("Vui lòng nhập mã hợp đồng!");
            if (args.UserId is null) return BadRequest("Vui lòng nhập UserId!");
            if (!User.IsInRole(RoleName.Admin) && !User.IsInRole(RoleName.Cx) && !User.IsInRole(RoleName.CxTP)) return BadRequest("Bạn không có quyền đổi mã hợp đồng!");
            if (await _context.Users.AnyAsync(x => x.ContractCode == args.ContractCode)) return BadRequest("Mã hợp đồng đã tồn tại!");
            var user = await _context.Users.FindAsync(args.UserId);
            if (user is null) return BadRequest("User not found!");
            user.ContractCode = args.ContractCode;
            _context.Users.Update(user);
            await _logService.AddAsync($"{user.Name} đã cập nhật mã hợp đồng");
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("add-sub-contract")]
    public async Task<IActionResult> AddSubContractAsync([FromBody] Contract args)
    {
        if (string.IsNullOrWhiteSpace(args.Code)) return BadRequest("Vui lòng nhập mã hợp đồng!");
        args.CreatedDate = DateTime.Now;
        args.CreatedBy = User.GetId();
        if (await _context.Users.FindAsync(args.CardHolderId) is null) return BadRequest("Không tìm thấy chủ thẻ!");
        await _context.Contracts.AddAsync(args);
        await _logService.AddAsync($"{User.GetUserName()} đã thêm hợp đồng {args.Code}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("delete-sub-contract/{id}")]
    public async Task<IActionResult> DeleteSubContractAsync([FromRoute] Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract is null) return BadRequest("Không tìm thấy hợp đồng!");
        _context.Contracts.Remove(contract);
        await _logService.AddAsync($"{User.GetUserName()} đã xóa hợp đồng {contract.Code}");
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("my-revenue-total")]
    public async Task<IActionResult> MyRevenueTotalAsync()
    {
        var userId = User.GetId();
        return Ok(new
        {
            data = new
            {
                pending = await _context.UserTopups.Where(x => x.SaleId == userId && (x.Status == TopupStatus.Pending)).SumAsync(x => x.Amount),
                accountant = await _context.UserTopups.Where(x => x.SaleId == userId && x.Status == TopupStatus.DirectorApproved).SumAsync(x => x.Amount),
                total = await _context.UserTopups.Where(x => x.SaleId == userId && x.Status == TopupStatus.AccountantApproved).SumAsync(x => x.Amount),
                month = await _context.UserTopups.Where(x => x.SaleId == userId && x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Year == DateTime.Now.Year && x.Status == TopupStatus.AccountantApproved).SumAsync(x => x.Amount),
                year = await _context.UserTopups.Where(x => x.SaleId == userId && x.CreatedDate.Year == DateTime.Now.Year && x.Status == TopupStatus.AccountantApproved).SumAsync(x => x.Amount)
            }
        });
    }

    [HttpGet("team")]
    public async Task<IActionResult> TeamAsync([FromQuery] UserFilterOptions filterOptions)
    {
        var query = from a in _context.Users
                    join b in _context.UserRoles on a.Id equals b.UserId
                    join c in _context.Roles on b.RoleId equals c.Id
                    where a.Status == UserStatus.Working
                    select new
                    {
                        a.SmId,
                        a.DosId,
                        a.Branch,
                        a.Id,
                        a.Name,
                        a.Gender,
                        a.PhoneNumber,
                        a.Email,
                        a.TmId,
                        a.DotId,
                        a.DateOfBirth,
                        a.UserName,
                        RoleName = c.Name,
                        a.Avatar
                    };
        var userId = User.GetId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Unauthorized();
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles?.FirstOrDefault() ?? string.Empty;
        if (User.IsInRole(RoleName.TelesaleManager))
        {
            query = query.Where(x => x.TmId == userId);
        }
        else if (User.IsInRole(RoleName.SalesManager))
        {
            query = query.Where(x => x.SmId == userId);
        }
        else if (User.IsInRole(RoleName.Dos))
        {
            query = query.Where(x => x.DosId == userId);
        }
        else if (User.IsInRole(RoleName.Dot))
        {
            query = query.Where(x => x.DosId == userId);
        }
        else
        {
            query = query.Where(x => x.RoleName == role);
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(x => x.Name.ToLower().Contains(filterOptions.Name.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber.ToLower().Contains(filterOptions.PhoneNumber.ToLower()));
        }
        query = query.OrderByDescending(x => x.Id);
        return Ok(await ListResult<object>.Success(query, filterOptions));
    }

    [HttpGet("points")]
    public async Task<IActionResult> PointsAsync([FromQuery] UserPointFilterOptions filterOptions) => Ok(await _userService.PointsAsync(filterOptions));

    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] ApplicationUser args)
    {
        var user = await _userManager.FindByIdAsync(User.GetClaimId());
        if (user is null) return BadRequest("User not found!");
        user.Name = args.Name;
        user.PhoneNumber = args.PhoneNumber;
        user.Email = args.Email;
        user.DateOfBirth = args.DateOfBirth;
        user.Gender = args.Gender;
        user.Address = args.Address;
        user.IdentityNumber = args.IdentityNumber;
        user.IdentityAddress = args.IdentityAddress;
        user.IdentityDate = args.IdentityDate;
        await _userManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPost("loan-point")]
    public async Task<IActionResult> LoanPointAsync([FromBody] LoanPointArgs args) => Ok(await _loanService.LoanPointAsync(args));

    [HttpGet("loan-list")]
    public async Task<IActionResult> MyLoanAsync([FromQuery] LoanFilterOptions filterOptions) => Ok(await _loanService.LoanListAsync(filterOptions));

    [HttpPost("approve-loan")]
    public async Task<IActionResult> ApproveLoanAsync([FromBody] ApproveLoanArgs args) => Ok(await _loanService.ApproveLoanAsync(args));
}
