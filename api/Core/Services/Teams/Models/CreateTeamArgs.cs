namespace Waffle.Core.Services.Teams.Models;

public class CreateTeamArgs
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = default!;
}
