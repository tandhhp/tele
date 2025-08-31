using Waffle.Core.Interfaces.IRepository.Tele;
using Waffle.Core.Interfaces.IService.Tele;
using Waffle.Models;

namespace Waffle.Core.Services.Tele;

public class CallStatusService(ICallStatusRepository _callStatusRepository) : ICallStatusService
{
    public Task<object> OptionsAsync(SelectOptions options) => _callStatusRepository.OptionsAsync(options);
}
