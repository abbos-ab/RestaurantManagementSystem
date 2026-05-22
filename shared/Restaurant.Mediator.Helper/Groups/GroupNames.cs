namespace Restaurant.Mediator.Helper.Groups;

public static class GroupNames
{
    /// <summary>
    /// Full access to all system features and management functions.
    /// </summary>
    public const string Administrators = "Administrators";

    /// <summary>
    /// Responsible for serving customers, managing orders, and handling tables.
    /// </summary>
    public const string Waiters = "Waiters";

    /// <summary>
    /// Responsible for preparing meals, managing kitchen operations, and handling food preparation.
    /// </summary>
    public const string Chefs = "Chefs";

    /// <summary>
    /// Restaurant customer who can browse menus, place orders, and make reservations.
    /// </summary>
    public const string Customers = "Customers";
}