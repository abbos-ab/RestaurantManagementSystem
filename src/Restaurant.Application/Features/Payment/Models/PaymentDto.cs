using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Payment.Models;

public sealed record PaymentDto(
    long Id,
    long OrderId,
    decimal Amount,
    PaymentMethod Method,
    long WaiterId,
    PaymentStatus Status
);