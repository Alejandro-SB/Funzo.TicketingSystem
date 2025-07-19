using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TicketingSystem.Controllers;
using TicketingSystem.Persistence;
using TicketingSystem.Persistence.Entities;
using TicketingSystem.UseCases.Users;

namespace TicketingSystem.Tests;
public class UsersTests
{
    private readonly CustomWebAppFactory _factory = new();

    [Fact]
    public async Task CreateUser_Creates_New_User()
    {
        var client = _factory.CreateClient();
        var request = new CreateUserHttp
        {
            Username = "ValidUsername",
            DisplayName = "Valid user name"
        };

        var response = await client.PostAsJsonAsync("/users", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<CreateUserResult>();

        Assert.NotNull(result);
        Assert.False(result.IsErr(out _));
    }

    [Fact]
    public async Task CreateUser_Fails_If_Username_Is_Empty()
    {
        var client = _factory.CreateClient();
        var request = new CreateUserHttp
        {
            Username = "",
            DisplayName = "Valid user name"
        };

        var response = await client.PostAsJsonAsync("/users", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<CreateUserResult>();

        Assert.NotNull(result);
        Assert.True(result.IsErr(out var err));

        Assert.True(err.Is<UsernameNotValid>(out _));
    }

    [Fact]
    public async Task CreateUser_Fails_If_DisplayName_Is_Empty()
    {
        var client = _factory.CreateClient();
        var request = new CreateUserHttp
        {
            Username = "ValidUsername",
            DisplayName = ""
        };

        var response = await client.PostAsJsonAsync("/users", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<CreateUserResult>();

        Assert.NotNull(result);
        Assert.True(result.IsErr(out var err));

        Assert.True(err.Is<DisplayNameNotValid>(out _));
    }

    [Fact]
    public async Task CreateUser_Fails_If_Username_Already_Exists()
    {
        var username = "Username";
        await CreateUser(username);

        var client = _factory.CreateClient();
        var request = new CreateUserHttp
        {
            Username = username,
            DisplayName = "Valid display name"
        };

        var response = await client.PostAsJsonAsync("/users", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.FromJson<CreateUserResult>();

        Assert.NotNull(result);
        Assert.True(result.IsErr(out var err));

        Assert.True(err.Is<UserNameAlreadyExists>(out _));
    }

    private async Task<int> CreateUser(string username)
    {
        var user = new User
        {
            Username = username,
            DisplayName = username
        };

        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();

        return user.Id;
    }
}
