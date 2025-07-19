using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;
using TicketingSystem.UseCases.Tickets.Common;

namespace TicketingSystem.UseCases.Tickets;

public class SolveTicket : IUseCase<SolveTicketRequest, SolveTicketResult>
{
    private readonly AppDbContext _context;

    public SolveTicket(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SolveTicketResult> Handle(SolveTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return new TicketNotFound();
        }

        if (ticket.ResolutionDate is { } resolutionDate)
        {
            return new TicketAlreadySolved(resolutionDate);
        }

        ticket.ResolutionDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return SolveTicketResult.Ok();
    }
}

public record SolveTicketRequest(int TicketId);

[Result<SolveTicketError>]
public partial class SolveTicketResult;

[Union<TicketNotFound, TicketAlreadySolved>]
public partial class SolveTicketError;