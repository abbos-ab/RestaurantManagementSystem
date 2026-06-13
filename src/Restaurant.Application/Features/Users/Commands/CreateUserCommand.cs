using Ardalis.Specification;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    List<long> GroupIds
) : ICommand<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Password)
            .MinimumLength(6);
    }
}

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;
    private readonly TimeProvider _timeProvider;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IGroupRepository _groupRepository;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        UserMapper userMapper,
        TimeProvider timeProvider,
        IPasswordHasher<User> passwordHasher,
        IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
        _timeProvider = timeProvider;
        _passwordHasher = passwordHasher;
        _groupRepository = groupRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserByEmailSpec(request.Email);
        var exists = await _userRepository.AnyAsync(spec, cancellationToken);
        if (exists)
            throw new BusinessLogicException(UserErrors.AlreadyExists);

        var phone = PhoneNumber.Create(request.PhoneNumber);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = phone,
            Password = "",
            IsActive = true,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password);

        var groupSpec = new DbSpecification<Group>();
        groupSpec.Query.Where(x => request.GroupIds.Contains(x.Id));

        var groups = await _groupRepository.ListAsync(groupSpec, cancellationToken);

        if (groups.Count != request.GroupIds.Count)
            throw new ResourceNotFoundException(new Error("Group.NotFound", "One or more groups were not found"));
        
        foreach (var group in groups)
        {
            user.Groups.Add(group);
        }

        await _userRepository.AddAsync(user, cancellationToken);

        return _userMapper.Map(user);
    }
}