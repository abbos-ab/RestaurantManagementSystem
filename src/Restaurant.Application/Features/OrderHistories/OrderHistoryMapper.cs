using Restaurant.Application.Features.OrderHistories.Commands;
using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.OrderHistories;

[Mapper]
public partial class OrderHistoryMapper
{
    public partial OrderHistoryDto Map(OrderHistory entity);

    public partial List<OrderHistoryDto> Map(List<OrderHistory> entities);
}