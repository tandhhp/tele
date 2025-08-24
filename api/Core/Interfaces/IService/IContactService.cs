using Waffle.Core.Services.Contacts.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface IContactService
{
    Task<TResult> BlockAsync(BlockContactArgs args);
    public Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions);
}
