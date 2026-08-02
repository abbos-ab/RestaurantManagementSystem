using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Restaurant.Application.Features.Dishes.Commands;
using Restaurant.Application.Features.Dishes.Models;

namespace Restaurant.IntegrationTests.DishController;

public class CreateDishControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    private readonly List<long> _createdIds = new();

    public CreateDishControllerTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Create_Return_Ok_WhenDishIsCreated()
    {
        // Arrange
        var command = new CreateDishCommand(
            Name: "Pizza",
            CategoryId: 6,
            Price: 10,
            Description: "Pizza test",
            IsActive: true
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/Dishes/Create", command);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<DishDto>();
        _createdIds.Add(customerResponse!.Id);
        customerResponse.CategoryId.Should().Be(command.CategoryId);
        customerResponse.Description.Should().Be(command.Description);
        customerResponse.Should().BeEquivalentTo(command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}