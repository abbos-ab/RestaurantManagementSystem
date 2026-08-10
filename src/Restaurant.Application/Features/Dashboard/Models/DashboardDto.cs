using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Application.Features.Dashboard.Models;

public sealed class DashboardDto
{
    public int TodayOrders { get; set; }

    public decimal TodayRevenue { get; set; }

    public int PendingOrders { get; set; }

    public int PreparingOrders { get; set; }

    public int ReadyOrders { get; set; }

    public int CompletedOrders { get; set; }

    public int CancelledOrders { get; set; }

    public int TotalTables { get; set; }

    public int OccupiedTables { get; set; }

    public int AvailableTables { get; set; }
    public int TodayReviews { get; set; }

    public double AverageRating { get; set; }
    public int ActiveWaiters { get; set; }

    public int ActiveChefs { get; set; }
}