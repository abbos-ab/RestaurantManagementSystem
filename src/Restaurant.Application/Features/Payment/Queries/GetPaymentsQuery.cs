using Ardalis.Specification;
using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Payment.Queries;

public sealed record GetPaymentsQuery(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<PaymentDto>>;

internal sealed class GetPaymentsQueryHandler : IQueryHandler<GetPaymentsQuery, PaginatedResult<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentMapper _mapper;

    public GetPaymentsQueryHandler(IPaymentRepository paymentRepository, PaymentMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Domain.Entities.Payment>();
        spec.Query
            .Include(x => x.Order)
            .Include(x => x.Waiter)
            .WithPagination(request.PaginationInfo);

        var payments = await _paymentRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _paymentRepository.CountAsync(spec, cancellationToken);

        var mapperPayments = _mapper.Map(payments);

        return new PaginatedResult<PaymentDto>(mapperPayments, totalCount);
    }
}