using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;
using TicketingSystem.Persistence.Entities;

namespace TicketingSystem.UseCases.Tickets;

public class CreateTicket : IUseCase<CreateTicketRequest, CreateTicketResult>
{
    private readonly AppDbContext _context;

    public CreateTicket(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTicketResult> Handle(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Subject))
        {
            return new InvalidTicketSubject("Subject cannot be empty");
        }

        if (string.IsNullOrEmpty(request.Body) || request.Body.Length < Ticket.MinBodyChars)
        {
            return new InvalidTicketBody("Body should contain at least 50 characters.");
        }

        var existsUser = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!existsUser)
        {
            return new UserNotFound();
        }

        var ticket = new Ticket
        {
            Subject = request.Subject,
            Comments = [new Comment
            {
                Date = DateTime.UtcNow,
                Text = request.Body,
                TicketId = 0,
                UserId = request.UserId
            }]
        };

         _context.Add(ticket);

        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}


public record CreateTicketRequest(int UserId, string Subject, string Body);

[Result<int, CreateTicketError>]
public partial class CreateTicketResult;

[Union<InvalidTicketSubject, InvalidTicketBody, UserNotFound>]
public partial class CreateTicketError;
public record InvalidTicketSubject(string Reason);
public record InvalidTicketBody(string Reason);