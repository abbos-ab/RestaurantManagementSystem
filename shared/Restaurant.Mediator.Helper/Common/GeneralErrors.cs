using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Mediator.Helper.Common;

public static class GeneralErrors
{
    public static readonly Error Unauthorized = new(
        "General.Unauthorized",
        "Failed to retrieve the current user."
    );

    public static readonly Error AccessDenied = new(
        "General.AccessDenied",
        "Access denied."
    );

    public static Error CreationError(string message)
    {
        return new Error(
            "Create.Error",
            $"An error occurred while creating the entity. {message}"
        );
    }

    public static Error UpdateError(string message)
    {
        return new Error(
            "Update.Error",
            $"An error occurred while updating the entity. {message}"
        );
    }

    public static Error DeleteError(string message)
    {
        return new Error(
            "Delete.Error",
            $"An error occurred while deleting the entity. {message}"
        );
    }
}
