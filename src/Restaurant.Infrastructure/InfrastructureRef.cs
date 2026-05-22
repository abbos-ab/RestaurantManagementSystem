using System.Reflection;

namespace Restaurant.Infrastructure;

public static class InfrastructureRef
{
    public static Assembly Assembly => typeof(InfrastructureRef).Assembly;
}
