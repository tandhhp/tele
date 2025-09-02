using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Calls;
using Waffle.Data;
using Waffle.Entities.Contacts;

namespace Waffle.Infrastructure.Repositories.Calls;

public class CallHistoryRepository(ApplicationDbContext context) : EfRepository<CallHistory>(context), ICallHistoryRepository
{
}
