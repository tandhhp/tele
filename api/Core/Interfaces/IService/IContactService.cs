using Waffle.Core.Services.Contacts.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface IContactService
{
    Task<TResult> BlockAsync(BlockContactArgs args);
    Task<TResult> CreateContactAsync(CreateContactArgs args);
    Task<Contact?> FindAsync(Guid id);
    public Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions);
}
