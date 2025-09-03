using Waffle.Core.Interfaces.IRepository.Calls;
using Waffle.Core.Interfaces.IService.Calls;
using Waffle.Models;

namespace Waffle.Core.Services.Calls;

public class CallStatusService(ICallStatusRepository _callStatusRepository) : ICallStatusService
{
    public Task<object> OptionsAsync(SelectOptions options) => _callStatusRepository.OptionsAsync(options);
}
