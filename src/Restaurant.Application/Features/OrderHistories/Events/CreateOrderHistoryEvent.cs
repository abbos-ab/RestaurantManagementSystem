    using FluentValidation;
    using Restaurant.Application.Features.OrderHistories.Repositories;
    using Restaurant.Domain.Entities;
    using Restaurant.Mediator.Helper.Events;

    namespace Restaurant.Application.Features.OrderHistories.Events;

    public sealed record CreateOrderHistoryEvent(
        long OrderId,
        OrderHistoryAction Action,
        string Description,
        long? UserId,
        long? OrderItemId
    ) : IEvent;

    // ReSharper disable once UnusedType.Global
    public sealed class CreateOrderHistoryEventValidator : AbstractValidator<CreateOrderHistoryEvent>
    {
        public CreateOrderHistoryEventValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Order Id must be greater than 0");

            RuleFor(x => x.Description)
                .NotEmpty();
        }
    }

    internal sealed class CreateOrderHistoryEventHandler : IEventHandler<CreateOrderHistoryEvent>
    {
        private readonly IOrderHistoryRepository _historyRepository;
        private readonly TimeProvider _timeProvider;

        public CreateOrderHistoryEventHandler(
            IOrderHistoryRepository historyRepository,
            TimeProvider timeProvider)
        {
            _historyRepository = historyRepository;
            _timeProvider = timeProvider;
        }

        public async Task Handle(CreateOrderHistoryEvent request, CancellationToken cancellationToken)
        {
            var orderHistory = new OrderHistory
            {
                OrderId = request.OrderId,
                Action = request.Action,
                Description = request.Description,
                UserId = request.UserId,
                OrderItemId = request.OrderItemId,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
            };

            await _historyRepository.AddAsync(orderHistory, cancellationToken);
        }
    }