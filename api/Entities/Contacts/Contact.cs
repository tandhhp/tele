using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.Contacts;

public class Contact : BaseEntity
{
    [StringLength(450)]
    public string? Name { get; set; }
    [StringLength(450)]
    public string? Email { get; set; }
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    [StringLength(1000)]
    public string? Note { get; set; }
    [StringLength(500)]
    public string? Address { get; set; }
    public string? Meta { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? RefId { get; set; }
    public ContactStatus Status { get; set; }

    public List<ContactActivity>? Activities { get; set; }
}

public enum ContactStatus
{
    New,
    Blacklisted
}

public class ContactMeta
{
    public ContactMeta()
    {
        ErrorMessage = string.Empty;
    }
    public string ErrorMessage { get; set; }
}
