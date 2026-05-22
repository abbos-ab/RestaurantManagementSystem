using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record DeleteUserCommand(long UserId) : ICommand;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new BusinessLogicException(UserErrors.NotFound);

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}