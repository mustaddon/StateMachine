using System;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal static partial class TaskExt
{
#if NET6_0_OR_GREATER
    public static T GetResult<T>(this Task task) => task is Task<T> t ? t.Result : (T)((dynamic)task).Result;
#else
    public static T GetResult<T>(this Task task) => task is Task<T> t ? t.Result : (T)task.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(task);
#endif

    public static bool IsGeneric(this Task task)
    {
        var type = task.GetType();
        return type.IsGenericType && type.GetGenericArguments()[0].Name != "VoidTaskResult";
    }
}
