
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService.Tele;

public interface ICallStatusService
{
    Task<object> OptionsAsync(SelectOptions? options);
}
