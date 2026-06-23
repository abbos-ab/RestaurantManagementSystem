using System.Reflection;

namespace NotificationService.Application;

public static class ApplicationRef
{
    public static Assembly Assembly => typeof(ApplicationRef).Assembly;
}