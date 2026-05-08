using Restaurant.Application.Features.Categories.Commands;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Categories;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto Map(Category category);

    public partial List<CategoryDto> Map(List<Category> categories);

    public partial Category ToEntity(CreateCategoryCommand command);

    public partial void Map(UpdateCategoryCommand command, Category category);
}