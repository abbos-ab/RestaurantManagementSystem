using Auth.Application.UsersGroups;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Features.Users;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Application.Features.UsersGroups.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.UsersGroups.Commands;

public sealed record AssignUsersToGroupCommand(long GroupId, List<long> UserIds) : ICommand;

// ReSharper disable once UnusedType.Global
public sealed class AssignUsersToGroupCommandValidator : AbstractValidator<AssignUsersToGroupCommand>
{
    public AssignUsersToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);

        RuleFor(x => x.UserIds)
            .Must(x => x.Count > 0)
            .WithMessage("At least one user must be assigned to the group.");
    }
}

internal sealed class AssignUsersToGroupCommandHandler : ICommandHandler<AssignUsersToGroupCommand>
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignUsersToGroupCommandHandler> _logger;

    public AssignUsersToGroupCommandHandler(
        IUserGroupRepository userGroupRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignUsersToGroupCommandHandler> logger)
    {
        _userGroupRepository = userGroupRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AssignUsersToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.FirstOrDefaultAsync(
            new GroupByIdSpec(request.GroupId),
            cancellationToken
        );

        if (group is null)
            throw new BusinessLogicException(UserGroupErrors.NotFound);

        var users = await _userRepository.ListAsync(
            new UsersByIdsSpec(request.UserIds),
            cancellationToken
        );

        if (users.Count == 0)
            throw new BusinessLogicException(UserErrors.NotFound);

        var currentRelations = await _userGroupRepository.ListAsync(
            new UserGroupRelationsByGroupIdSpec(request.GroupId),
            cancellationToken
        );

        var currentUserIds = 
            currentRelations.Select(x => x.UserId).ToHashSet();

        var newUserIds = users.Select(x => x.Id).ToHashSet();

        var toAdd = newUserIds
            .Where(userId => !currentUserIds.Contains(userId))
            .Select(userId => new UserGroupRelation
            {
                GroupId = group.Id,
                UserId = userId
            }).ToList();

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (toAdd.Count > 0)
                await _userGroupRepository.AddRangeAsync(toAdd, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Group relations updated.");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogError(e, "Failed to update group relations.");

            throw new BusinessLogicException(
                GeneralErrors.UpdateError("Failed to update group relations."));
        }
    }
}