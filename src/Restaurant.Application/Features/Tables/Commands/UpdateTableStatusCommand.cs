using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Commands;

public sealed record UpdateTableStatusCommand(long TableId, TableStatus Status) : ICommand<TableDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateTableStatusCommandValidator : AbstractValidator<UpdateTableStatusCommand>
{
    public UpdateTableStatusCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}

internal sealed class UpdateTableStatusCommandHandler : ICommandHandler<UpdateTableStatusCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;

    public UpdateTableStatusCommandHandler(ITableRepository tableRepository, TableMapper tableMapper)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
    }

    public async Task<TableDto> Handle(UpdateTableStatusCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        table.Status = request.Status;
        await _tableRepository.UpdateAsync(table, cancellationToken);
        
        return _tableMapper.Map(table);
    }
}