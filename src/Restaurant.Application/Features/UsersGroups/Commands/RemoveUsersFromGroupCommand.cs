using Auth.Application.UsersGroups;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Features.Users;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Application.Features.UsersGroups.Specifications;
using Restaurant.Mediator.Helper.Common;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.UsersGroups.Commands;

public sealed record RemoveUsersFromGroupCommand(long GroupId, List<long> UserIds) : ICommand;

// ReSharper disable once UnusedType.Global
internal sealed class RemoveUsersFromGroupCommandValidator : AbstractValidator<RemoveUsersFromGroupCommand>
{
    public RemoveUsersFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
        RuleFor(x => x.UserIds)
            .Must(x => x.Count > 0)
            .WithMessage("At least one user must be specified.");
    }
}

internal sealed class RemoveUserFromGroupCommandHandler : ICommandHandler<RemoveUsersFromGroupCommand>
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveUserFromGroupCommandHandler> _logger;

    public RemoveUserFromGroupCommandHandler(
        IUserGroupRepository userGroupRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveUserFromGroupCommandHandler> logger)
    {
        _userGroupRepository = userGroupRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(RemoveUsersFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.FirstOrDefaultAsync(
            new GroupByIdSpec(request.GroupId),
            cancellationToken
        );

        if (group is null)
            throw new BusinessLogicException(UserGroupErrors.Group.NotFound);

        var users = await _userRepository.ListAsync(
            new UsersByIdsSpec(request.UserIds),
            cancellationToken
        );

        if (users.Count == 0)
            throw new BusinessLogicException(UserErrors.NotFound);

        var currentRelations = await _userGroupRepository.ListAsync(
            new UserGroupRelationsByGroupIdSpec(group.Id),
            cancellationToken
        );

        var userIds = request.UserIds.ToHashSet();

        var toDelete = currentRelations
            .Where(x => userIds.Contains(x.UserId))
            .ToList();

        if (toDelete.Count == 0)
        {
            _logger.LogInformation("User not found");
            return;
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _userGroupRepository.DeleteRangeAsync(toDelete, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Group relations deleted.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogError(ex, "Failed to remove users from group.");

            throw new BusinessLogicException(
                GeneralErrors.DeleteError("Failed to remove users from group."));
        }
    }
}
