using Microsoft.EntityFrameworkCore;
using Waffle.Entities;

namespace Waffle.Models;

public class LoginModel
{
    public string? UserName { get; set; }
    public string Password { get; set; } = default!;
    public bool IsAdmin { get; set; }
}

public class ChangePasswordModel : BaseEntity
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}

public class ExportDateFilterOptions
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
public class CreateUserModel : LoginModel
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int Loyalty { get; set; }
    public bool? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
    public string Name { get; set; } = default!;
    public Guid CardId { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? IdentityDate { get; set; }
    public string? IdentityAddress { get; set; }
    public string? HealthHistory { get; set; }
    public string? FamilyCharacteristics { get; set; }
    public string? Personality { get; set; }
    public string? Concerns { get; set; }
    public string? ContractCode { get; set; }
    public string Role { get; set; } = default!;
    public Guid? SallerId { get; set; }
    public Guid? DosId { get; set; }
    public Guid? SmId { get; set; }
    public Branch1 Branch { get; set; }
    public int? MaxLoyalty { get; set; }
    // Convert từ lead sang
    public bool LeadConvert { get; set; }
    public Guid? LeadId { get; set; }
    public DateTime? ContractDate { get; set; }
    public Guid? TmId { get; set; }
    public Guid? TrainerId { get; set; }
}

public class AddToRoleModel : BaseEntity
{
    public string RoleName { get; set; } = default!;
}

public class RemoveFromRoleModel : BaseEntity
{
    public string RoleName { get; set; } = default!;
}
