using Restaurant.Application.Features.Tables.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Tables;

[Mapper]
public partial class TableMapper
{
    public partial TableDto Map(Table entity);

    public partial List<TableDto> Map(List<Table> entities);
}