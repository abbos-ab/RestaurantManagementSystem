using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Payment.Commands;
using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

public class PaymentController : BaseController
{
    public PaymentController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<PaymentDto> Create(
        CreatePaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpGet]
    public async Task<PaginatedResult<PaymentDto>> GetAll(
        PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPaymentsQuery(paginationInfo), cancellationToken);
    }

    [HttpGet("{paymentId:long}")]
    public async Task<PaymentDto> GetById(
        long paymentId,
        [FromQuery] GetPaymentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPaymentByIdQuery(paymentId), cancellationToken);
    }

    [HttpPut("{paymentId:long}")]
    public async Task<PaymentDto> Update(
        long paymentId,
        UpdatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new UpdatePaymentCommand(
                paymentId,
                request.Amount,
                request.Method,
                request.Status
            ),
            cancellationToken
        );
    }

    [HttpPut("{paymentId:long}/status")]
    public async Task<PaymentDto> UpdateStatus(
        long paymentId,
        PaymentStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new UpdatePaymentStatusCommand(paymentId, status), cancellationToken);
    }

    [HttpDelete("{paymentId:long}")]
    public async Task<bool> Delete(
        long paymentId, 
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new DeletePaymentCommand(paymentId), cancellationToken);
    }
}