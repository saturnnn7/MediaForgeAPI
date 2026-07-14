using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediaForge.Gateway.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Gateway.IntegrationTests.Tests;

[Collection("Gateway")]
public sealed class NotificationHubTests(RabbitMqFixture fixture)
{
    [Fact]
    public async Task NotificationHubSendsMessageToCorrectUser()
    {
        var testUserId = Guid.NewGuid();
        await using var factory = new GatewayFactory(fixture);
        using var client = factory.CreateClient();

        await using var hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost/hubs/notifications", opts =>
            {
                opts.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
                opts.AccessTokenProvider = () => Task.FromResult<string?>(GenerateTestJwt(testUserId));
                opts.Transports = HttpTransportType.LongPolling;
            })
            .Build();

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        hubConnection.On<object>("MediaProcessingCompleted", payload => tcs.SetResult(payload));

        await hubConnection.StartAsync();
        await Task.Delay(500);

        var bus = factory.Services.GetRequiredService<IBus>();
        await bus.Publish(new MediaProcessingCompletedEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            testUserId,
            "transcription text",
            ["https://example.com/output.mp4"],
            "https://example.com/thumb.jpg",
            42));

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(10_000));
        completed.Should().Be(tcs.Task, "notification should arrive within 10 seconds");

        await hubConnection.StopAsync();
    }

    private static string GenerateTestJwt(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-signing-key-32-bytes-long!!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: [new Claim("sub", userId.ToString())],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
