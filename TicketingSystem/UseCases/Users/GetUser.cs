using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;

namespace TicketingSystem.UseCases.Users;

public class GetUser : IUseCase<GetUserRequest, Option<GetUserResponse>>
{
    private readonly AppDbContext _context;

    public GetUser(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Option<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Where(u => u.Id == request.Id)
            .Select(u => new GetUserResponse(u.Id, u.Username, u.DisplayName))
            .FirstOrDefaultAsync(cancellationToken);

        return Option.FromValue(user);
    }
}


public record GetUserRequest(int Id);

public record GetUserResponse(int Id, string Username, string DisplayName);