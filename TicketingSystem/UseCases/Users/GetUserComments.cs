using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;

namespace TicketingSystem.UseCases.Users;

public class GetUserComments : IUseCase<GetUserCommentsRequest, GetUserCommentsResponse>
{
    private readonly AppDbContext _context;

    public GetUserComments(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetUserCommentsResponse> Handle(GetUserCommentsRequest request, CancellationToken cancellationToken)
    {
        var userComments = await _context.Comments.Where(c => c.UserId == request.UserId)
            .Select(c => new UserComment(c.TicketId, c.Text, c.Date))
            .ToListAsync(cancellationToken);

        return new GetUserCommentsResponse(userComments);
    }
}

public record GetUserCommentsRequest(int UserId);

public record GetUserCommentsResponse(IEnumerable<UserComment> Comments);

public record UserComment(int TicketId, string Text, DateTimeOffset Date);