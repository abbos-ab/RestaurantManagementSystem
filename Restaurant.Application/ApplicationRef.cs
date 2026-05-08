using System.Reflection;

namespace Restaurant.Application;

public static class ApplicationRef
{
    public static Assembly Assembly => typeof(ApplicationRef).Assembly;
}
