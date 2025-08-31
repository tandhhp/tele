using Microsoft.EntityFrameworkCore;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IRepository;
using Waffle.Core.Services.Contacts.Models;
using Waffle.Data;
using Waffle.Entities.Contacts;
using Waffle.Models;

namespace Waffle.Infrastructure.Repositories;

public class ContactRepository(ApplicationDbContext context) : EfRepository<Contact>(context), IContactRepository
{
    public async Task<ListResult<object>> GetBlacklistAsync(BlacklistFilterOptions filterOptions)
    {
        var query = from c in _context.Contacts
                    where c.Status == ContactStatus.Blacklisted
                    select new
                    {
                        c.Id,
                        c.Name,
                        c.Email,
                        c.PhoneNumber,
                        c.Note,
                        c.Address,
                        c.CreatedDate,
                        c.Status
                    };
        if (!string.IsNullOrWhiteSpace(filterOptions.Name))
        {
            query = query.Where(c => c.Name.Contains(filterOptions.Name, StringComparison.CurrentCultureIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(filterOptions.PhoneNumber))
        {
            query = query.Where(c => c.PhoneNumber.Contains(filterOptions.PhoneNumber));
        }
        query = query.OrderByDescending(c => c.CreatedDate);
        return await ListResult<object>.Success(query, filterOptions);
    }

    public Task<bool> IsPhoneExistAsync(string phoneNumber) => _context.Contacts.AnyAsync(x => x.PhoneNumber == phoneNumber);
}
