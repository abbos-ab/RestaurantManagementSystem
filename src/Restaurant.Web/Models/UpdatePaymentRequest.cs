using Restaurant.Domain.Entities;

namespace Restaurant.Web.Models;

public record UpdatePaymentRequest(
    decimal Amount,
    PaymentMethod Method,
    PaymentStatus Status
);