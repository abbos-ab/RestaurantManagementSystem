using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Users;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "User not found"
    );

    public static readonly Error AlreadyExists = new(
        "User.AlreadyExists",
        "User already exists"
    );

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "Invalid credentials"
    );

    public static readonly Error Inactive = new(
        "User.Inactive",
        "User is inactive"
    );
}