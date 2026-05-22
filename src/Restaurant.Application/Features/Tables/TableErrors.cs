using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Tables;

public static class TableErrors
{
    public static readonly Error NotFound = new(
        "Table.NotFound",
        "Table not found"
    );

    public static readonly Error AlreadyExists = new(
        "Table.AlreadyExists",
        "Table already exists"
    );
    
    public static readonly Error TableOccupied = new(
        "Table.Occupied",
        "Table is occupied"
    );
    
    public static readonly Error TableReserved = new(
        "Table.Reserved",
        "Table is reserved"
    );
    
    public static readonly Error TableDisabled = new(
        "Table.Disabled",
        "Table is disabled"
    );
}