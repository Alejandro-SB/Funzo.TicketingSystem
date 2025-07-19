using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;
using TicketingSystem.Persistence.Entities;
using TicketingSystem.UseCases.Tickets.Common;

namespace TicketingSystem.UseCases.Tickets;

public class AddCommentToTicket : IUseCase<AddCommentToTicketRequest, AddCommentToTicketResult>
{
    private readonly AppDbContext _context;

    public AddCommentToTicket(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AddCommentToTicketResult> Handle(AddCommentToTicketRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return new InvalidCommentText("Text cannot be empty");
        }

        var existsUser = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!existsUser)
        {
            return new UserNotFound();
        }

        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return new TicketNotFound();
        }

        if (ticket.ResolutionDate is { } resolutionDate)
        {
            return new TicketAlreadySolved(resolutionDate);
        }

        _context.Add(new Comment
        {
            UserId = request.UserId,
            TicketId = request.TicketId,
            Date = DateTimeOffset.UtcNow,
            Text = request.Text
        });

        await _context.SaveChangesAsync(cancellationToken);

        return AddCommentToTicketResult.Ok();
    }
}

public record AddCommentToTicketRequest(int TicketId, string Text, int UserId);

[Result<AddCommentToTicketError>]
public partial class AddCommentToTicketResult;


[Union<TicketNotFound, UserNotFound, InvalidCommentText, TicketAlreadySolved>]
public partial class AddCommentToTicketError;

public record UserNotFound;
public record InvalidCommentText(string Reason);