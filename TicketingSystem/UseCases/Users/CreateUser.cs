using Funzo;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Persistence;
using TicketingSystem.Persistence.Entities;

namespace TicketingSystem.UseCases.Users;

public class CreateUser : IUseCase<CreateUserRequest, CreateUserResult>
{
    private readonly AppDbContext _context;

    public CreateUser(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(request.Username))
        {
            return new UsernameNotValid("Username should not be empty");
        }

        if(string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return new DisplayNameNotValid("Display name should not be empty");
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if(existingUser is not null)
        {
            return new UserNameAlreadyExists();
        }

        var user = new User
        {
            Username = request.Username,
            DisplayName = request.DisplayName,
        };

        _context.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}

public record CreateUserRequest(string Username, string DisplayName);

[Result<int, CreateUserError>]
public partial class CreateUserResult;

[Union<UserNameAlreadyExists, UsernameNotValid, DisplayNameNotValid>]
public partial class CreateUserError;

public record UserNameAlreadyExists;

public record UsernameNotValid(string Reason);
public record DisplayNameNotValid(string Reason);