using Waffle.Core.Interfaces;

namespace Waffle.Services;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _httpClient;
    public TelegramService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            var token = "1017931181:AAG2pumlDqYBXn8GINp99Cq_e6lk23YuVWg";
            var chatId = "-4209488594";
            string url = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&text={message}&parse_mode=HTML&disable_web_page_preview=true";

            await _httpClient.GetAsync(url);
        }
        catch (Exception)
        {

        }
    }
}
