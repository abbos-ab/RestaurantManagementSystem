using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.IntegrationTests.DishController;

public class GetDishControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _factory;

    public GetDishControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Return_NotFound_WhenDishDoesntExist()
    {
        // Act
        var response = await _factory.GetAsync("/api/Dishes/GetById/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var text = await response.Content.ReadAsStringAsync();
        text.Should().Contain("Dish not found");

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Be("Dish not found");
        problem.Status.Should().Be(404);
    }
    
    [Fact]
    public async Task Get_Return_Ok_WhenDishExists()
    {
        // Act
        var response = await _factory.GetAsync("/api/Dishes/GetById/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customerResponse = await response.Content.ReadFromJsonAsync<DishDto>();
        customerResponse!.Name.Should().Be("Lavash");
    }
    
    [Fact]
    public async Task Get_Return_Ok_WhenDishExists_WithName()
    {
        string name = "Lavash";
        int index = 0;
        int size = 5;

        var response = await _factory.GetAsync($"/api/Dishes/SearchByName?name={name}&index={index}&size={size}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<DishDto>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThan(0);

        var dish = result.Items.Single(x => x.Name == "Lavash");

        dish.Name.Should().Be("Lavash");
        dish.CategoryId.Should().Be(1);
        dish.Price.Should().Be(35);
        dish.Description.Should().Be("Chicken lavash");
        dish.IsActive.Should().BeTrue();
    }
}