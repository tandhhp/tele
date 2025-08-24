using Waffle.Core.Services.KeyIn.Models;
using Waffle.Models;

namespace Waffle.Core.Interfaces.IService;

public interface IKeyInService
{
    Task<TResult> UpdateBranchAsync(UpdateBranchArgs args);
}
