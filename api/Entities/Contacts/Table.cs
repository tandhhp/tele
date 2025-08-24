namespace Waffle.Entities.Contacts;

public class Table
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Floor { get; set; } = default!;
    public bool? Available { get; set; }
    public bool Active { get; set; }
    public int SortOrder { get; set; }
    public Branch1? Branch { get; set; }

    public List<LeadFeedback>? LeadFeedbacks { get; set; }
}
