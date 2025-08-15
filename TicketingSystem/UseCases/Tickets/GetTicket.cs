using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;

namespace TicketingSystem.UseCases.Tickets;

public class GetTicket : IUseCase<GetTicketRequest, Option<GetTicketResponse>>
{
    private readonly AppDbContext _context;

    public GetTicket(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Option<GetTicketResponse>> Handle(GetTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .Where(t => t.Id == request.Id)
            .Select(t => new GetTicketResponse(t.Id, t.Subject, t.IsEscalated, t.ResolutionDate, t.Comments.Select(c => new TicketComment(c.UserId, c.User.DisplayName, c.Text))))
            .FirstOrDefaultAsync(cancellationToken);

        return Option.FromValue(ticket);
    }
}

public record GetTicketRequest(int Id);
public record TicketComment(int UserId, string DisplayName, string Text);
public record GetTicketResponse(int Id, string Subject, bool IsEscalated, DateTimeOffset? ResolutionDate, IEnumerable<TicketComment> Comments);