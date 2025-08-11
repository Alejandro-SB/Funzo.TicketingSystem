using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;

namespace TicketingSystem.UseCases.Users;

public record GetAllUsersRequest;

public record GetAllUsersResponse(IEnumerable<GetAllUsersUser> Users);
public record GetAllUsersUser(int Id, string Username, string DisplayName, int UserComments);

public class GetAllUsers : IUseCase<GetAllUsersRequest, GetAllUsersResponse>
{
    private readonly AppDbContext _appDbContext;

    public GetAllUsers(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<GetAllUsersResponse> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await _appDbContext.Users.Select(x => new GetAllUsersUser(x.Id, x.Username, x.DisplayName, x.Comments.Count())).ToListAsync(cancellationToken);

        return new GetAllUsersResponse(users);
    }
}
