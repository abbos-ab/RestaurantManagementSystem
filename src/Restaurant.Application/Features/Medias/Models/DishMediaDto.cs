using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Medias.Models;

public sealed record DishMediaDto
{
    public long Id { get; set; }
    
    public required MediaType MediaType { get; set; }
    
    public required string Path { get; set; }
    
    public double FileWidth { get; set; }
    
    public double FileHeight { get; set; }
    
    public double FileSize { get; set; }
}