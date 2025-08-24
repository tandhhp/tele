namespace Waffle.Core.Interfaces
{
    public interface ICurrentUser
    {
        Guid GetId();
        bool IsInRole(string role);
    }
}
