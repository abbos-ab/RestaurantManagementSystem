using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Commands;

public sealed record UpdateTableCapacityCommand(long TableId, int Capacity) : ICommand<TableDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateTableCapacityCommandValidator : AbstractValidator<UpdateTableCapacityCommand>
{
    public UpdateTableCapacityCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("The TableId must be greater than 0");
        
        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("The Capacity must be greater than 0");
    }
}

internal sealed class UpdateTableCapacityCommandHandler : ICommandHandler<UpdateTableCapacityCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;

    public UpdateTableCapacityCommandHandler(ITableRepository tableRepository, TableMapper tableMapper)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
    }

    public async Task<TableDto> Handle(UpdateTableCapacityCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        table.Capacity = request.Capacity;
        await _tableRepository.UpdateAsync(table, cancellationToken);
        
        return _tableMapper.Map(table);
    }
}
