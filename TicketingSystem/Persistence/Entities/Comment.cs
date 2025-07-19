namespace TicketingSystem.Persistence.Entities;

public class Comment : BaseEntity
{
    public required string Text { get; set; }
    public required int TicketId { get; set; }
    public required int UserId { get; set; }
    public required DateTimeOffset Date { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User User { get; set; } = null!;
}
