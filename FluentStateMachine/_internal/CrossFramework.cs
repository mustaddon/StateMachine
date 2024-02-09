using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal static class CrossFramework
{

#if NET45
    public static readonly Task CompletedTask = Task.FromResult(false);
#else
    public static readonly Task CompletedTask = Task.CompletedTask;
#endif

#if NET6_0_OR_GREATER
    public static object GetResult(this Task task) => ((dynamic)task).Result;
#else
    public static object GetResult(this Task task) => task.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(task);
#endif

}
