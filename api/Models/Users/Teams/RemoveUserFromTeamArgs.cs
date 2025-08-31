namespace Waffle.Models.Users.Teams;

public class RemoveUserFromTeamArgs
{
    public int TeamId { get; set; }
    public Guid UserId { get; set; }
}
