namespace Waffle.Core.Interfaces
{
    public interface ITelegramService
    {
        Task SendMessageAsync(string message);
    }
}
