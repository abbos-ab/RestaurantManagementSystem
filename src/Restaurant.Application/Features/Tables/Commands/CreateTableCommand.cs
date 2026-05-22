using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Application.Features.Tables.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Commands;

public sealed record CreateTableCommand(int Number) : ICommand<TableDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0)
            .WithMessage("Number must be greater than 0");
    }
}

internal sealed class CreateTableCommandHandler : ICommandHandler<CreateTableCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;
    private readonly TimeProvider _timeProvider;

    public CreateTableCommandHandler(
        ITableRepository tableRepository,
        TableMapper tableMapper,
        TimeProvider timeProvider)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
        _timeProvider = timeProvider;
    }

    public async Task<TableDto> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var spec = new TableByNumberSpec(request.Number);
        var exists = await _tableRepository.AnyAsync(spec, cancellationToken);

        if (exists)
            throw new BusinessLogicException(TableErrors.AlreadyExists);

        var table = new Table
        {
            Number = request.Number,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };

        await _tableRepository.AddAsync(table, cancellationToken);

        return _tableMapper.Map(table);
    }
}