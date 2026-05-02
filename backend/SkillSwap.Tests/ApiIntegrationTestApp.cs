using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkillSwap.Api.Controllers;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Persistence;

namespace SkillSwap.Tests;

internal sealed class ApiIntegrationTestApp : IAsyncDisposable
{
    private readonly WebApplication _app;

    public HttpClient Client { get; }

    private ApiIntegrationTestApp(WebApplication app, HttpClient client)
    {
        _app = app;
        Client = client;
    }

    public static async Task<ApiIntegrationTestApp> StartAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "integration-test-secret-key-with-enough-length",
            ["Jwt:Issuer"] = "SkillSwap.Tests",
            ["Jwt:Audience"] = "SkillSwap.Tests",
            ["Jwt:ExpireMinutes"] = "60"
        });

        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(AuthController).Assembly);

        var databaseRoot = new InMemoryDatabaseRoot();
        var databaseName = Guid.NewGuid().ToString();

        builder.Services.AddDbContext<SkillSwapDbContext>(options =>
            options.UseInMemoryDatabase(databaseName, databaseRoot));

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services
            .AddAuthentication(TestAuthHandler.TestScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.TestScheme,
                _ => { });
        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Urls.Add("http://127.0.0.1:0");

        await app.StartAsync();

        var addresses = app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()!
            .Addresses;

        var client = new HttpClient
        {
            BaseAddress = new Uri(addresses.Single())
        };

        return new ApiIntegrationTestApp(app, client);
    }

    public async Task SeedAsync(Func<SkillSwapDbContext, Task> seed)
    {
        await using var scope = _app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SkillSwapDbContext>();
        await seed(context);
        await context.SaveChangesAsync();
    }

    public async Task<T> ReadContextAsync<T>(Func<SkillSwapDbContext, Task<T>> read)
    {
        await using var scope = _app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SkillSwapDbContext>();
        return await read(context);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _app.DisposeAsync();
    }
}

internal static class HttpClientTestAuthExtensions
{
    public static void AuthenticateAs(this HttpClient client, int userId, string email = "user@test.com")
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.TestScheme);
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        client.DefaultRequestHeaders.Remove(TestAuthHandler.EmailHeader);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, email);
    }
}

internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestScheme = "Test";
    public const string UserIdHeader = "X-Test-UserId";
    public const string EmailHeader = "X-Test-Email";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userId))
            return Task.FromResult(AuthenticateResult.NoResult());

        var email = Request.Headers.TryGetValue(EmailHeader, out var emailHeader)
            ? emailHeader.ToString()
            : $"user{userId}@test.com";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "user")
        };

        var identity = new ClaimsIdentity(claims, TestScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
