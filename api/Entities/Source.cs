using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities;

public class Source : AuditEntity
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
    [ForeignKey(nameof(SourceType))]
    public int SourceTypeId { get; set; }
    [StringLength(512)]
    public string? Note { get; set; }

    public virtual SourceType? SourceType { get; set; }
}
