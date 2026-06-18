using Restaurant.Application.Features.Payment.Models;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Payment;

[Mapper]
public partial class PaymentMapper
{
    public partial PaymentDto Map(Domain.Entities.Payment payment);
    
    public partial List<PaymentDto> Map(List<Domain.Entities.Payment> payments);
}