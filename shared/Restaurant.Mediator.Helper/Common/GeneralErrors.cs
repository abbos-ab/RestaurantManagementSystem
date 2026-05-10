using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Mediator.Helper.Common;

public static class GeneralErrors
{
    public static Error CreationError(string message)
    {
        return new Error(
            "Create.Error",
            $"An error occurred while creating. {message}"
        );
    }

    public static Error UpdateError(string message)
    {
        return new Error(
            "Update.Error",
            $"An error occurred while updating. {message}"
        );
    }

    public static Error DeleteError(string message)
    {
        return new Error(
            "Delete.Error",
            $"An error occurred while deleting. {message}"
        );
    }
}
