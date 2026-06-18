using Ardalis.Specification;

namespace Restaurant.Application.Features.Payment.Specifications;

public class PaymentByOrderIdSpec : Specification<Domain.Entities.Payment>
{
    public long OrderId;

    public PaymentByOrderIdSpec(long orderId, bool asNoTracking = false)
    {
        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.OrderId == orderId);
    }
}