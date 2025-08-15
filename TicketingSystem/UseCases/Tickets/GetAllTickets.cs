
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;

namespace TicketingSystem.UseCases.Tickets;

public class GetAllTickets : IUseCase<GetAllTicketsRequest, GetAllTicketsResponse>
{
    private readonly AppDbContext _context;

    public GetAllTickets(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllTicketsResponse> Handle(GetAllTicketsRequest request, CancellationToken cancellationToken)
    {
        var tickets = await _context.Tickets.Select(t => new GetAllTicketsTicket(t.Id, t.Subject, t.Comments.Count)).ToListAsync(cancellationToken);

        return new GetAllTicketsResponse(tickets);
    }
}


public record GetAllTicketsRequest;
public record GetAllTicketsTicket(int Id, string Subject, int TotalComments);

public record GetAllTicketsResponse(IEnumerable<GetAllTicketsTicket> Tickets);