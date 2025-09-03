using Waffle.Core.Services.Calls.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService.Calls;

public interface ICallStatusService
{
    Task<object> OptionsAsync(SelectOptions options);
}
