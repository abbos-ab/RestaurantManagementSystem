using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Commands;

public sealed record UpdateTableCommand(
    long TableId,
    int Number
) : ICommand<TableDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateTableCommandValidator : AbstractValidator<UpdateTableCommand>
{
    public UpdateTableCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");

        RuleFor(x => x.Number)
            .GreaterThan(0)
            .WithMessage("Number must be greater than 0");
    }
}

internal sealed class UpdateTableCommandHandler : ICommandHandler<UpdateTableCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;

    public UpdateTableCommandHandler(
        ITableRepository tableRepository,
        TableMapper tableMapper)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
    }

    public async Task<TableDto> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var entity = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (entity is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        entity.Number = request.Number;

        await _tableRepository.UpdateAsync(entity, cancellationToken);

        return _tableMapper.Map(entity);
    }
}