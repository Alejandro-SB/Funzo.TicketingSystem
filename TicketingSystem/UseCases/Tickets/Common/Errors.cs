namespace TicketingSystem.UseCases.Tickets.Common;

public record TicketNotFound;
public record TicketAlreadySolved(DateTimeOffset ResolutionDate);
