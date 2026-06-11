using FluentValidation;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.UsersGroups.Commands;

public sealed record CreateGroupCommand(string Name, string Description) : ICommand;

// ReSharper disable once UnusedType.Global
public sealed class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}

internal sealed class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly TimeProvider _timeProvider;

    public CreateGroupCommandHandler(
        IGroupRepository groupRepository,
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _timeProvider = timeProvider;
    }

    public async Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        await _groupRepository.AddAsync(
            new Group
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
            },
            cancellationToken
        );

        await _groupRepository.SaveChangesAsync(cancellationToken);
    }
}
