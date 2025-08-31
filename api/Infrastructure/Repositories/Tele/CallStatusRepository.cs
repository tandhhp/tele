using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository.Tele;
using Waffle.Data;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories.Tele;

public class CallStatusRepository(ApplicationDbContext context) : EfRepository<CallStatus>(context), ICallStatusRepository
{
    public async Task<object> OptionsAsync(SelectOptions options) => await _context.CallStatuses.Select(x => new
    {
        Label = x.Name,
        Value = x.Id
    }).ToListAsync();
}
