
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
        var tickets = await _context.Tickets.Select(t => new GetTicketResponse(t.Id, t.Subject, t.Body)).ToListAsync(cancellationToken);

        return new GetAllTicketsResponse(tickets);
    }
}


public record GetAllTicketsRequest;

public record GetAllTicketsResponse(IEnumerable<GetTicketResponse> Tickets);