using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Authentications;

public static class AuthErrors
{
    public static readonly Error UserIsDisabled = new(
        "Auth.UserIsDisabled",
        "User is disabled."
    );

    public static readonly Error DoesNotExist = new(
        "User.DoesNotExist",
        "User does not exist."
    );

    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Invalid username or password."
    );

    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "Unauthorized."
    );

    public static readonly Error PasswordShouldBeNew = new(
        "Auth.PasswordShouldBeNew",
        "Password must be different from the previous one."
    );

    public static readonly Error InvalidPhoneNumber = new(
        "Auth.InvalidPhoneNumber",
        "Invalid phone number format."
    );
}