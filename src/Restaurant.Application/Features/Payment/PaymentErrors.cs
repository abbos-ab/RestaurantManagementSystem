using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Payment;

public static class PaymentErrors
{
    public static readonly Error NotFound = new(
        "Payment.NotFound",
        "Payment not found"
    );

    public static readonly Error InvalidAmount = new(
        "Payment.InvalidAmount",
        "Payment amount must be greater than zero."
    );

    public static readonly Error AlreadyPaid = new(
        "Payment.AlreadyPaid",
        "Payment has already been completed."
    );

    public static readonly Error CannotDeletePaidPayment = new(
        "Payment.CannotDeletePaidPayment",
        "Paid payments cannot be deleted."
    );
}