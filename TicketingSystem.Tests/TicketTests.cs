using Funzo;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TicketingSystem.Controllers;
using TicketingSystem.Persistence;
using TicketingSystem.Persistence.Entities;
using TicketingSystem.UseCases.Tickets;
using TicketingSystem.UseCases.Tickets.Common;

namespace TicketingSystem.Tests;

public class TicketTests
    : IClassFixture<CustomWebAppFactory>
{
    private readonly CustomWebAppFactory _factory;

    public TicketTests(CustomWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTicket_Returns_None()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tickets/0");

        response.EnsureSuccessStatusCode();

        var maybeTicket = await response.Content.FromJson<Option<GetTicketResponse>>();

        Assert.False(maybeTicket.IsSome(out _));
    }

    [Fact]
    public async Task GetTicket_Returns_Existing_Ticket()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();

        var response = await client.GetAsync($"/api/tickets/{ticketId}");

        response.EnsureSuccessStatusCode();

        var maybeTicket = await response.Content.FromJson<Option<GetTicketResponse>>();

        Assert.True(maybeTicket.IsSome(out var ticket));
        Assert.Equal(ticketId, ticket.Id);
    }

    [Fact]
    public async Task CreateTicket_Creates_Ticket()
    {
        var client = _factory.CreateClient();
        var userId = await CreateUser();
        var ticketRequest = new CreateTicketHttpRequest
        {
            Subject = "I have a problem",
            Body = "Please help me. I cannot continue to the next screen. Something has happened that is not allowing me to move forward. Please help thanks!",
            UserId = userId
        };
        var response = await client.PostAsJsonAsync("/api/tickets", ticketRequest);

        response.EnsureSuccessStatusCode();

        var ticketResult = await response.Content.FromJson<CreateTicketResult>();

        Assert.NotNull(ticketResult);

        Assert.False(ticketResult.IsErr(out _, out _));
    }

    [Fact]
    public async Task CreateTicket_Returns_Err_With_No_Subject()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tickets", new CreateTicketHttpRequest
        {
            Subject = "",
            Body = "Please help me. I cannot continue to the next screen. Something has happened that is not allowing me to move forward. Please help thanks!"
        });

        response.EnsureSuccessStatusCode();

        var ticketResult = await response.Content.FromJson<CreateTicketResult>();

        Assert.True(ticketResult.IsErr(out var error));
        Assert.True(error.Is<InvalidTicketSubject>(out _));
    }

    [Fact]
    public async Task CreateTicket_Returns_Err_With_Invalid_User_Id()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tickets", new CreateTicketHttpRequest
        {
            Subject = "This is a subject",
            Body = "Please help me. I cannot continue to the next screen. Something has happened that is not allowing me to move forward. Please help thanks!"
        });

        response.EnsureSuccessStatusCode();

        var ticketResult = await response.Content.FromJson<CreateTicketResult>();

        Assert.True(ticketResult.IsErr(out var error));
        Assert.True(error.Is<UserNotFound>(out _));
    }

    [Fact]
    public async Task CreateTicket_Returns_Err_With_No_Body()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tickets", new CreateTicketHttpRequest
        {
            Subject = "I have a problem",
            Body = ""
        });

        response.EnsureSuccessStatusCode();

        var ticketResult = await response.Content.FromJson<CreateTicketResult>();

        Assert.True(ticketResult.IsErr(out var error));
        Assert.True(error.Is<InvalidTicketBody>(out _));
    }

    [Fact]
    public async Task EscalateTicket_Returns_Err_When_Ticket_Not_Found()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/tickets/0/escalate", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<EscalateTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketNotFound>(out _));
    }

    [Fact]
    public async Task EscalateTicket_Returns_Err_When_Ticket_Already_Resolved()
    {
        var client = _factory.CreateClient();
        var newTicketId = await CreateTicketAndGetId();
        await Solve(newTicketId);
        var response = await client.PostAsync($"/api/tickets/{newTicketId}/escalate", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<EscalateTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketAlreadySolved>(out _));
    }

    [Fact]
    public async Task EscalateTicket_Returns_Err_When_Ticket_Already_Escalated()
    {
        var client = _factory.CreateClient();
        var newTicketId = await CreateTicketAndGetId();
        await Escalate(newTicketId);

        var response = await client.PostAsync($"/api/tickets/{newTicketId}/escalate", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<EscalateTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketAlreadyEscalated>(out _));
    }

    [Fact]
    public async Task EscalateTicket_Escalates_Created_Ticket()
    {
        var client = _factory.CreateClient();
        var newTicketId = await CreateTicketAndGetId();

        var response = await client.PostAsync($"/api/tickets/{newTicketId}/escalate", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<EscalateTicketResult>();

        Assert.False(result.IsErr(out _));
    }

    [Fact]
    public async Task AddCommentToTicket_Adds_A_Comment_To_The_Ticket()
    {
        var ticketId = await CreateTicketAndGetId();
        var userId = await CreateUser();
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new AddCommentHttpRequest
        {
            Text = "Comment",
            UserId = userId
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<AddCommentToTicketResult>();

        Assert.False(result.IsErr(out _));
    }

    [Fact]
    public async Task AddCommentToTicket_When_Ticket_Does_Not_Exists_Returns_Err()
    {
        var client = _factory.CreateClient();
        var userId = await CreateUser();

        var response = await client.PostAsJsonAsync($"/api/tickets/0/comments", new AddCommentHttpRequest
        {
            Text = "Comment",
            UserId = userId
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<AddCommentToTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketNotFound>(out _));
    }

    [Fact]
    public async Task AddCommentToTicket_When_User_Does_Not_Exists_Returns_Err()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new AddCommentHttpRequest
        {
            Text = "Comment",
            UserId = 0
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<AddCommentToTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<UserNotFound>(out _));
    }

    [Fact]
    public async Task AddCommentToTicket_When_Comment_Is_Not_Valid_Returns_Err()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();
        var userId = await CreateUser();

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new AddCommentHttpRequest
        {
            Text = string.Empty,
            UserId = userId
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<AddCommentToTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<InvalidCommentText>(out _));
    }

    [Fact]
    public async Task AddCommentToTicket_When_Ticket_Is_Already_Solved_Returns_Err()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();
        var userId = await CreateUser();
        await Solve(ticketId);

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new AddCommentHttpRequest
        {
            Text = "Valid text to comment",
            UserId = userId
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<AddCommentToTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketAlreadySolved>(out _));
    }

    [Fact]
    public async Task SolveTicket_Marks_Ticket_As_Solved()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();

        var response = await client.PostAsync($"/api/tickets/{ticketId}/solve", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<SolveTicketResult>();

        Assert.False(result.IsErr(out _));
    }

    [Fact]
    public async Task SolveTicket_Returns_Err_When_Ticket_Not_Found()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync($"/api/tickets/0/solve", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<SolveTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketNotFound>(out _));
    }

    [Fact]
    public async Task SolveTicket_Returns_Err_When_Ticket_Is_Already_Solved()
    {
        var client = _factory.CreateClient();
        var ticketId = await CreateTicketAndGetId();
        await Solve(ticketId);

        var response = await client.PostAsync($"/api/tickets/{ticketId}/solve", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<SolveTicketResult>();

        Assert.True(result.IsErr(out var err));
        Assert.True(err.Is<TicketAlreadySolved>(out _));
    }

    private async Task<int> CreateTicketAndGetId()
    {
        using var scope = _factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ticket = new Ticket
        {
            Subject = "I have a problem",
            Body = "Please help me. I cannot continue to the next screen. Something has happened that is not allowing me to move forward. Please help thanks!",
            UserId = 0
        };

        context.Tickets.Add(ticket);

        await context.SaveChangesAsync();

        return ticket.Id;
    }

    private async Task Solve(int ticketId)
    {
        using var scope = _factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        ticket.ResolutionDate = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();
    }

    private async Task Escalate(int ticketId)
    {
        using var scope = _factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        ticket.IsEscalated = true;

        await context.SaveChangesAsync();
    }

    private async Task<int> CreateUser()
    {
        using var scope = _factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = new User
        {
            Username = "username",
            DisplayName = "username"
        };

        context.Add(user);
        await context.SaveChangesAsync();

        return user.Id;
    }
}
