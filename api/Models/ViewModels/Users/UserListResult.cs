using Waffle.Entities;

namespace Waffle.Models.ViewModels.Users;

public class UserListResult
{
    public Guid Id { get; set; }
    public Guid? CardId { get; set; }
    public string? CardCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? Gender { get; set; }
    public int Loyalty { get; set; }
    public string? Name { get; set; }
    public string? UserName { get; set; }
    public Tier? Tier { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
}
