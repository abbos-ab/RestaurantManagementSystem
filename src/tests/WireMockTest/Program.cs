using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

var wireMockServer = WireMockServer.Start(new WireMockServerSettings
{
    Urls = [ "http://localhost:8080"]
});

Console.WriteLine(wireMockServer.Url);

wireMockServer.Given(Request.Create()
        .WithPath("/example")
        .UsingGet())
    .RespondWith(Response.Create()
        .WithBody("This coming from WireMick")
        .WithStatusCode(200)
        .WithHeaders(new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "Content-Type", "application/json; charset=utf-8" }
        })
        .WithStatusCode(HttpStatusCode.BadGateway));

Console.ReadKey();
wireMockServer.Stop();