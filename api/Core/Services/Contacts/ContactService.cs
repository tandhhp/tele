using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Contacts.Models;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Services.Contacts;

public class ContactService(IContactRepository _contactRepository, ILogService _logService) : IContactService
{
    public async Task<TResult> BlockAsync(BlockContactArgs args)
    {
        var contact = await _contactRepository.FindAsync(args.Id);
        if (contact is null) return TResult.Failed("Không tìm thấy liên hệ!");
        contact.Status = ContactStatus.Blacklisted;
        contact.Note = args.Note;
        await _contactRepository.UpdateAsync(contact);
        await _logService.AddAsync($"Chặn liên hệ {contact.Name} - {contact.PhoneNumber}");
        return TResult.Success;
    }

    public Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions) => _contactRepository.GetBlacklistAsync(filterOptions);
}
