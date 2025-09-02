using Waffle.Core.Interfaces.IRepository.Calls;
using Waffle.Core.Interfaces.IService;
using Waffle.Core.Interfaces.IService.Calls;
using Waffle.Core.Services.Calls.Models;
using Waffle.Models;

namespace Waffle.Core.Services.Calls;

public class CallStatusService(ICallStatusRepository _callStatusRepository, IContactService _contactService) : ICallStatusService
{
    public async Task<TResult> CompleteAsync(CallCompleteArgs args)
    {
        var contact = await _contactService.FindAsync(args.ContactId);
        if (contact is null) return TResult.Failed("Không tìm thấy liên hệ!");
        return TResult.Success;
    }

    public Task<object> OptionsAsync(SelectOptions options) => _callStatusRepository.OptionsAsync(options);
}
