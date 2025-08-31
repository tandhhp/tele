using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository.Tele;

public interface ICallStatusRepository : IAsyncRepository<CallStatus>
{
    Task<object> OptionsAsync(SelectOptions options);
}
