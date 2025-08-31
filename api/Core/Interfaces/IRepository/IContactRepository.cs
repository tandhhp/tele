using Waffle.Core.Services.Contacts.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IRepository;

public interface IContactRepository : IAsyncRepository<Contact>
{
    Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions);
    Task<bool> IsPhoneExistAsync(string phoneNumber);
}
