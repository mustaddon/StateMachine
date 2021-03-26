using System.Threading.Tasks;

namespace RandomSolutions
{
    static class FrameworkExt
    {
#if NET45
        internal static readonly Task CompletedTask = Task.FromResult(false);
#else
        internal static readonly Task CompletedTask = Task.CompletedTask;
#endif
    }
}
