using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal static partial class TaskExt
{
#if NET6_0_OR_GREATER
    public static object GetResult(this Task task) => ((dynamic)task).Result;
#else
    public static object GetResult(this Task task) => task.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(task);
#endif

}
