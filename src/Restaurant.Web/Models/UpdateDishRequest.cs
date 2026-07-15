namespace Restaurant.Web.Models;

public record UpdateDishRequest(
    string? Name,
    long? CategoryId,
    string? Description
);