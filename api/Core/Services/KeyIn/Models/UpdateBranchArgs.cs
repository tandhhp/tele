using Waffle.Entities;

namespace Waffle.Core.Services.KeyIn.Models
{
    public class UpdateBranchArgs
    {
        public Guid KeyIn { get; set; }
        public Branch1 Branch { get; set; }
    }
}
