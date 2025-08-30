namespace Waffle.Models.Users;

public class CreateUserArgs
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Name { get; set; } = default!;
    public int? TeamId { get; set; }
}
