using System;

namespace Restaurant.Mediator.Helper.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetLocalDateTimeNowKindUtc(this TimeProvider timeProvider)
    {
        var dateTime = timeProvider.GetLocalNow().UtcDateTime;
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}