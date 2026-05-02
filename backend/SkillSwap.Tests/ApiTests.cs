using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SkillSwap.Tests;

public class ApiTests
{
    [Fact]
    public async Task SearchUsers_WithoutAuth_ReturnsUnauthorized()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        var response = await app.Client.GetAsync("/api/users/search");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidPassword_ReturnsBadRequest()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        var response = await app.Client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test",
            email = "test@test.com",
            password = "123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        await app.Client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test",
            email = "test@test.com",
            password = "Password1"
        });

        var response = await app.Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "test@test.com",
            password = "WrongPassword"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_NotExisting_ReturnsNotFound()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        app.Client.AuthenticateAs(1);

        var response = await app.Client.GetAsync("/api/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsTokenField()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        await app.Client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test",
            email = "test@test.com",
            password = "Password1"
        });

        var response = await app.Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "test@test.com",
            password = "Password1"
        });

        response.EnsureSuccessStatusCode();

        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        Assert.True(json.RootElement.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task SearchUsers_WithoutFilters_ReturnsOk()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        app.Client.AuthenticateAs(1);

        var response = await app.Client.GetAsync("/api/users/search");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatch_WithoutAuth_ReturnsUnauthorized()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();

        var response = await app.Client.PostAsync("/api/match/2", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Review_WithoutActiveMatch_ReturnsBadRequest()
    {
        await using var app = await ApiIntegrationTestApp.StartAsync();
        app.Client.AuthenticateAs(1);

        var response = await app.Client.PostAsJsonAsync("/api/review/user/2", new
        {
            rating = 5,
            comment = "Test"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}