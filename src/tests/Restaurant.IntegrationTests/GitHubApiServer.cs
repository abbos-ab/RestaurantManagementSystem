using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Restaurant.IntegrationTests;

public class GitHubApiServer : IDisposable
{
    private WireMockServer _server;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupUser(string userName)
    {
        _server.Given(Request.Create()
                .WithPath($"/users/{userName}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody("This is coming form WireMock")
                .WithHeader("Content-Type", "application/json;  charset=utf-8")
                .WithStatusCode(200));
    }
    
    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}