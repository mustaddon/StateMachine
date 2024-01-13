using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal static class FrameworkExt
{

#if NET45
    public static readonly Task CompletedTask = Task.FromResult(false);
#else
    public static readonly Task CompletedTask = Task.CompletedTask;
#endif

}
