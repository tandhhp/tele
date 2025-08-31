namespace Waffle.Models.Users.Teams;

public class AddUserToTeamArgs
{
    public int TeamId { get; set; }
    public Guid UserId { get; set; }
}
