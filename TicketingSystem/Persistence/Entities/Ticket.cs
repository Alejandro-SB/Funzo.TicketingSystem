namespace TicketingSystem.Persistence.Entities;

public class Ticket : BaseEntity
{
    public const int MinBodyChars = 50;

    public required string Subject { get; set; }
    public DateTimeOffset? ResolutionDate { get; set; }
    public bool IsEscalated { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
}
