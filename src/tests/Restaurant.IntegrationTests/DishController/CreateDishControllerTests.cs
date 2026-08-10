using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Restaurant.Application.Features.Dishes.Commands;
using Restaurant.Application.Features.Dishes.Models;

namespace Restaurant.IntegrationTests.DishController;

public class CreateDishControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public CreateDishControllerTests(CustomWebApplicationFactory applicationFactory)
    {
        _httpClient = applicationFactory.CreateClient();
    }

    [Fact]
    public async Task Create_Return_Ok_WhenDishIsCreated()
    {
        // Arrange
        var command = new CreateDishCommand(
            Name: "Pizza",
            CategoryId: 6,
            Description: "Pizza test",
            IsActive: true,
            Price: 12
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/Dishes/Create", command);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<DishDto>();
        customerResponse.Should().BeEquivalentTo(command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}