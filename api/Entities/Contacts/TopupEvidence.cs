using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Contacts;

public class TopupEvidence : BaseEntity
{
    [ForeignKey(nameof(UserTopup))]
    public Guid TopupId { get; set; }
    public string FileUrl { get; set; } = default!;

    public UserTopup? UserTopup { get; set; }
}
