using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waffle.Core.Helpers;
using Waffle.Core.Interfaces;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.Contacts.Models;
using Waffle.Entities;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Core.Services.Contacts;

public class ContactService(IContactRepository _contactRepository, ILogService _logService, UserManager<ApplicationUser> _userManager, ICurrentUser _currentUser) : IContactService
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

    public async Task<TResult> CreateContactAsync(CreateContactArgs args)
    {
        try
        {
            if (!PhoneNumberValidator.IsValidVietnamPhoneNumber(args.PhoneNumber)) return TResult.Failed("Số điện thoại không hợp lệ");
            if (await _contactRepository.IsPhoneExistAsync(args.PhoneNumber)) return TResult.Failed("Số điện thoại đã tồn tại");
            if (args.UserId != null && !await _userManager.Users.AnyAsync(x => x.Id == args.UserId)) return TResult.Failed("Người dùng không tồn tại");

            await _contactRepository.AddAsync(new Contact
            {
                Name = args.Name,
                PhoneNumber = args.PhoneNumber,
                Email = args.Email,
                Status = ContactStatus.New,
                CreatedDate = DateTime.Now,
                UserId = args.UserId,
                CreatedBy = _currentUser.GetId(),
                DistrictId = args.DistrictId,
                JobKindId = args.JobKindId,
                MarriedStatus = args.MarriedStatus,
                Note = args.Note,
                Gender = args.Gender,
                TransportId = args.TransportId
            });
            return TResult.Success;
        }
        catch (Exception ex)
        {
            return TResult.Failed(ex.ToString());
        }
    }

    public Task<Contact?> FindAsync(Guid id) => _contactRepository.FindAsync(id);

    public Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions) => _contactRepository.GetBlacklistAsync(filterOptions);
}
