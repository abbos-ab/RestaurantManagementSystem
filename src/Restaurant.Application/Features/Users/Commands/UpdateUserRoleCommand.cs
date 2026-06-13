using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record UpdateUserRoleCommand(long UserId, List<long> GroupIds) : ICommand<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0");

        RuleFor(x => x.GroupIds).NotEmpty();
    }
}

internal sealed class UpdateUserRoleCommandHadler : ICommandHandler<UpdateUserRoleCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly UserMapper _userMapper;

    public UpdateUserRoleCommandHadler(
        IUserRepository userRepository,
        UserMapper userMapper,
        IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
        _groupRepository = groupRepository;
    }

    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new BusinessLogicException(UserErrors.NotFound);
        
        var spec = new DbSpecification<Group>();
        spec.Query.Where(x => request.GroupIds.Contains(x.Id));
        var groups = await _groupRepository.ListAsync(spec, cancellationToken);

        if (!groups.Any())
            throw new ResourceNotFoundException(new Error("GroupErrors.NotFound", "Group not found"));

        user.Groups.Clear();
        
        foreach (var group in groups)
        {
            user.Groups.Add(group);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return _userMapper.Map(user);
    }
}