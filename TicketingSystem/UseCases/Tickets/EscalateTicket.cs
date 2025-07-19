using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;
using TicketingSystem.UseCases.Tickets.Common;

namespace TicketingSystem.UseCases.Tickets;

public class EscalateTicket : IUseCase<EscalateTicketRequest, EscalateTicketResult>
{
    private readonly AppDbContext _context;

    public EscalateTicket(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EscalateTicketResult> Handle(EscalateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if(ticket is null)
        {
            return new TicketNotFound();
        }

        if (ticket.ResolutionDate is { } resolutionDate)
        {
            return new TicketAlreadySolved(resolutionDate);
        }

        if(ticket.IsEscalated)
        {
            return new TicketAlreadyEscalated();
        }

        ticket.IsEscalated = true;

        await _context.SaveChangesAsync(cancellationToken);

        return EscalateTicketResult.Ok();
    }
}

public record EscalateTicketRequest(int Id);

[Result<EscalateTicketError>]
public partial class EscalateTicketResult;

[Union<TicketAlreadyEscalated, TicketNotFound, TicketAlreadySolved>]
public partial class EscalateTicketError;

public record TicketAlreadyEscalated;
