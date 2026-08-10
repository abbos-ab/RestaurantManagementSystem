using Restaurant.Application.Features.Dashboard.Models;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;

namespace Restaurant.Application.Features.Dashboard.Queries;

public sealed record GetDashboardQuery() : IQuery<DashboardDto>;

internal sealed class GetDashboardHandler
    : IQueryHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public GetDashboardHandler(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        ITableRepository tableRepository,
        IReviewRepository reviewRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _tableRepository = tableRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        return new DashboardDto
        {
            TodayOrders = await _orderRepository.GetTodayOrderCountAsync(
                today,
                cancellationToken),

            TodayRevenue = await _orderRepository.GetTodayRevenueAsync(
                today,
                cancellationToken),

            CompletedOrders = await _orderRepository.GetCompletedOrderCountAsync(
                cancellationToken),

            PendingOrders = await _orderItemRepository.CountByStatusAsync(
                OrderItemStatus.Pending,
                cancellationToken),

            PreparingOrders = await _orderItemRepository.CountByStatusAsync(
                OrderItemStatus.Preparing,
                cancellationToken),

            ReadyOrders = await _orderItemRepository.CountByStatusAsync(
                OrderItemStatus.Ready,
                cancellationToken),

            CancelledOrders = await _orderItemRepository.CountByStatusAsync(
                OrderItemStatus.Cancelled,
                cancellationToken),

            TotalTables = await _tableRepository
                .GetTotalTablesAsync(cancellationToken),

            OccupiedTables = await _tableRepository
                .GetOccupiedTablesAsync(cancellationToken),

            AvailableTables = await _tableRepository
                .GetAvailableTablesAsync(cancellationToken),

            TodayReviews = await _reviewRepository
                .GetTodayReviewsCountAsync(
                    today,
                    cancellationToken),

            AverageRating = await _reviewRepository
                .GetAverageRatingAsync(
                    cancellationToken),

            ActiveWaiters = await _userRepository
                .GetActiveWaitersCountAsync(
                    cancellationToken),

            ActiveChefs = await _userRepository
                .GetActiveChefsCountAsync(
                    cancellationToken),
        };
    }
}