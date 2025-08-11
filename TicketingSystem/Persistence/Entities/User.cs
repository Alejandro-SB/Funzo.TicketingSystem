namespace TicketingSystem.Persistence.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string DisplayName { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
}
