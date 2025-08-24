using Waffle.Core.Interfaces.IService;
using Waffle.Core.Services.KeyIn.Models;
using Waffle.Data;
using Waffle.Models;

namespace Waffle.Core.Services.KeyIn;

public class KeyInService : IKeyInService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;
    public KeyInService(ApplicationDbContext context, ILogService logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task<TResult> UpdateBranchAsync(UpdateBranchArgs args)
    {
        var keyIn = await _context.Leads.FindAsync(args.KeyIn);
        if (keyIn is null) return TResult.Failed("KeyIn not found");
        keyIn.Branch = args.Branch;
        _context.Leads.Update(keyIn);
        await _logService.AddAsync($"KeyIn {keyIn.Id} updated branch to {args.Branch}");
        await _context.SaveChangesAsync();
        return TResult.Success;
    }
}
